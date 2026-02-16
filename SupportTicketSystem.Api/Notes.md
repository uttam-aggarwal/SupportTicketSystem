# Support System Ticket Manager System (DotNet MVC)

We are creating a support system ticket manager system using **dotnet mvc**
**mvc** is models views controllers
* **models** is sort of data server that manages the data and give what we need or add what we want it to , it also process stuff for us
* then we have **controller** it is sort of the funtions that command or manages as a bridge between view it see request and decide what to do with that action whethher to send it to view or model and if it reciew from model then it tell to view what to do with it
* **view** is our display the interaction we do at the client side where it takes input and show the ui updates we want it to show

here in **supportticketsystem**
first we created a template using command

```bash
dotnet new sln -n SupportTicketSystem
dotnet new webapi -n SupportTicketSystem.Api
dotnet sln add SupportTicketSystem.Api
cd SupportTicketSystem.Api
```

this sln is a solution template sort of the holder where we will be adding other stuff , after that we create a webapi  then add it to our solution 

after that first we created a model of **user** which is what all things will be linked to one user and 
then we created our **applicationdb context**- it is a link to our database it fetches data from it or add required data in the database it may change and delete data from database too as required

**Db context** is a class provided by microsoft
One **DbContext** object = one “conversation session” with the database., this obj lets us delete insert update or read rows 

then we have this `dbset<t>` here **t** represent the type of row in that table in the database that we want to access using our code and the name after that like `dbset<User> Users` so here **User** is the type of row , this user is from the model->user.cs and is a class and the **users** is the table in the sql database witht he collection of user type objects

so this line `_context.Users.Add(user);`
tells the sql like `insert into users(...)`

so going into more of this db we have things like 

```csharp
var user = _context.Users.FirstOrDefault(u => u.Email == "a@test.com");
```

there user variable need something from the database
so `context.users` lets us communicate with the sql table 
then `.first` or default is us asking like give me first thing or data that matches my condition
then i ask for a email that matches the name i am giving , c# is smart so we need not to tell him here that u is user table as we are using methods like firstordefault or where it automatically fo to the list or table when we use lambda functions like this and as we are in tha particular thing it doesn;t make much sense to do your regualr stuff of lamda that doesn't involve that tables's row

next this ->
```csharp
_context.Users.Add(user);
```

here we tell users table that we are inserting a new row in your table called user but we are not actually inserting it here we just add it for the code part the actual insertion happen 

```csharp
await _context.SaveChangesAsync();
```

here we push all the pending insertion in the database

| Concern | What happens |
| :--- | :--- |
| **What does `await` do?** | Waits without blocking |
| **Will rerunning create duplicates?** | Yes, unless you check or enforce uniqueness |
| **What if program crashes mid-save?** | Database rolls back — no partial save |
| **Does EF protect me?** | Yes — via transactions |

**transcation** is sort of space where it is all put it one save changes put all the data it needs to add in a transcation and after it is finsished sending it says yes done then the data is saved but if it stops midway and the done call is not recieved that data never get sotred in the actual database 

### Simple analogy
**Think of ordering food:**
* **Without await:** You leave the restaurant before food arrives.
* **With await:** You wait for the food before leaving — but you can use your phone while waiting.

### What is a “server thread”?
A server thread is basically a worker.
Your server has a limited number of workers.
Each worker can handle one request at a time.
So if:
* 100 users send requests
* You have 20 workers
* → Only 20 requests can be actively processed at once.
The rest must wait.

### What happens WITHOUT await?
Let’s say User A sends a request to save data.
The server assigns:
👉 **Worker #1** to handle it.
**Worker #1:**
* Sends save request to database
* Waits doing nothing until DB responds
During this time, Worker #1 cannot help anyone else
So if DB takes 2 seconds:
* Worker #1 is blocked for 2 seconds
* Other users may have to wait even if their requests are quick

### What happens WITH await?
Same situation:
* User A sends request
* Worker #1 starts saving data
But now:
* Worker #1 pauses the method
* Worker #1 is freed and can help User B, C, D...
When the database finishes:
* The system gives Worker #1 (or another worker) the job back
* The method continues exactly where it paused
So:
The work continues later, but the worker isn’t stuck waiting.

### What method is being “held back”?
This part:
```csharp
await _context.SaveChangesAsync();
```
The current method (e.g., `Register()` or `Login()`) is paused.
So code like:
```csharp
await _context.SaveChangesAsync();
return Ok();
```
Means:
* Don’t `return Ok()` until save finishes
* But don’t block the worker while waiting

### 🧠 Final mental model

| Without await | With await |
| :--- | :--- |
| Worker waits doing nothing | Worker helps other users |
| Method blocks thread | Method pauses, thread is free |
| Server slows down | Server stays responsive |

# ApplicationDbContext Analysis

Now going to the line we are using in the **applicationdbcontext** of ours righ now that is 

```csharp
public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options) { } 
```

here focusing on `(dbcontextoptions<applicationdbcontext> option)` 
this **dbcontext options** tells the code about how you cna connect to the database that includes the location to connection the type of thing we are connecting to like whether it is sq; or swl lite or something we give it username and password and server name plus database name for the connection
we also give **behaviour setiings** (logging or tracking) etc
**behaviiour settings** is for the EF to know how to behave for example you may change tracking to query mode so it only access data when you explicitely asked it to , ou may turn on **sensitive data logging** which will show real values in log and error such as emails and password , this is useful for the debugging and development but should not be in the actual application
then there is **detialed error setting** or **lazy loading** or **custom command timeouts** and stuff , you use it like this

```csharp
options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

options.EnableSensitiveDataLogging();

options.EnableDetailedErrors();

options.UseLazyLoadingProxies();

options.CommandTimeout(60);
```

then going back to our **application db context** in that line we haven't tell how to connect as we mention all that in our **program.cs** file in the line

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

moving on to the next line

```csharp
public DbSet<User> Users=>Set<User>();
```

here we are using the **dbset** class and getting the **Users** to store the table of type **User**
like give me the table that stores users rows you may also use

```csharp
public DbSet<User> Users { get; set; }
```

this also works 

### Points to be know
points to be know , this cs file is just sort of a strucutre and currently is not fetching anything at all all the fetching ahpends when we use code like `_context.Users.firstordefault` and stuff this is just a strucutre that a table like this exists

# Program.cs Setup & Security Implementation

Do same with this and make sure you take care of your error with triple ticks

```csharp
var builder = WebApplication.CreateBuilder(args);
```

Here we are putting a builder as a variable that is having a ASP. net type we application , this is use to configure settings , register services and stuff

```csharp
builder.Services.AddControllers(); //tells that i will be using controllers in the app

builder.Services.AddEndpointsApiExplorer(); //This registers a service that scans your API endpoints.

builder.Services.AddSwaggerGen(); //Enable Swagger, so I can get a web UI that shows my API and lets me test it.
```

**Swagger** is a api testing tool sort of which list all your api endpoints related documents , it shows required inputs plus it lets you send text request and show ouputs hence it is great for testing your api endpoints

## Database Context Setup

Next->

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

Here the line `builder.Services.AddDbContext<ApplicationDbContext>(...)`
this sort of says whenever my application need dbcontext create and manage it , so now our controller can use `_context` , that is the bridge between database

```csharp
options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
```

This is letting it know that we are going to use **sql server** , as EF supports many other servers for you to use 
plus we are letting it know by `builder.Configuration.GetConnectionString("DefaultConnection")` that go into my configuration file and look for a string default connection that have all the info you need to connect to the database

So somewhere (usually in **appsettings.json**), you have:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=...;Database=...;User Id=...;Password=...;"
}
```

This string tells SQL Server:
* Where the server is
* Which database to use
* How to authenticate

```csharp
var app = builder.Build();
```

This says all setup done now build the app

# Password Hasher Script

Now we are going to see our password hasher script
we are hashing apssword and only storing that in the database so even if our database got compromised we are not exposiing raw user's passwords

```csharp
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
```

We are going ot use microsoft library to hash our password which is slow ,secure and industry level password hashing algorithm
the cryptography also helps us to generate random data which we will need to genrerate password hashes

```csharp
namespace SupportTicketSystem.Api.Helpers
{
    public static class PasswordHasher
    {
        ...
    }}
```

We are putting it in the helpers's folder or helper method of our api which will contains our simialr logical functions
then we are decalring it as  **public** so we can call it anywhere from our project plus **static** as it is just a utility and we don't need to create objects of it as we are not making data out of it put just using its methods to process stuff

## The Importance of Salt

Next 
but before code my take on password hashing and inportance of salt 
so lets say the hacker got something like `ajindando.aodnfas` right? , and they got `asdjmoadfak.asjojdfoa` right? now both of these are the same password in reality but no algorithm that haker may use can say that these are the smae thing adn the thing before the dot is waht is being used to genearet what is after the dot so there is a link between these and this is what we are storing in our database too all of this is okay so when a user sends a request to sign in after he is already registered and uses his passwrod we see his password then we bring this salt.hash gibberish frm our database use that salt we convert what user just send adn if the result is same as what is after dot we say login successful and with never putting his password there we are stopping the risk of exposure and even if he crack one fo the hsahes ans see that pswoord was 843993 something he doesn;t know who all are using the same password 

**IMPORTANT -> cryptography cannot use strings so it is necessary to convert it into bytes**

```csharp
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace supportTicketSystem.Api.Helpers
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            byte[] salt=RandomNumberGenerator.GetBytes(128/8);

            string hashed=Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password:password,
                salt:salt,
                prf:KeyDerivationPrf.HMACSHA256,
                iterationCount:100_100,
                numBytesRequested:256/8
            ));
            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }
        ...
    }}    
```

Here too we are defining salt using randomnumber generator which is giving us 128/8 that is 16 so it will give us 16 random bytes
next `keyderivation.pbkdf2` is a function from waht we imported it is slow and secure , slow is a functionlaity of it as it makes brute force attacking very difficult and with so much tierations it make tha tpossiblity almost 0;

* **HMAC** → Hash-based Message Authentication Code
* **SHA256** → Secure Hash Algorithm 256-bit

There is more variant such as sha1 and sha 512 with 512 being most secured but 256 is a good balance between security and speed

# JWT Implementation

Moving on to **JWT** so **jwt** is json web token
it's a sort of hotel's key card it contains the inforamtion like 
* who the user is 
* what role they have 
* that they are authenticated 
* that their login is still valid and stuff

Without jwt once the user signs in , ther server neeeds to remeber the session and check multiple things every time user send request
but with jwt , the user sends the token with every request which helps making application much more scalable

## AppSettings Configuration

Now in **appsettings.json** we are adding these settings related to our jwt

```json
"JwtSettings": {
  "Key": "THIS_IS_A_VERY_SECRET_KEY_CHANGE_IT",
  "Issuer": "SupportTicketSystem",
  "Audience": "SupportTicketSystemUsers",
  "ExpiresInMinutes": 60
}
```

Here **key** is a secret code that lets us check if hte token is genuine or not
second is **issuer** , who issued the key , if the key is not issue by us or th eapi we directly rejects it
so **audience** says that this token is valid only for this particular api if you want to rewust other apis you cannot with this token

## Program.cs JWT Setup
Now lets add our jwt setup in out **program.cs** file starting with neccessary libraries and functions

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
```

We will add these here:
* The first one `using Microsoft.AspNetCore.Authentication.JwtBearer`, gives us the ability to accept jwt token or identify them , without this application is unable to understand jwt at all and it give The `.AddJwtBearer()` method
* Second line `using Microsoft.IdentityModel.Tokens;` gives us the inspector that checks the incoming tokens
* `System.Text` This gives `Encoding.UTF8.GetBytes(...)` Which we need to Convert your secret key string into raw bytes so cryptography can use it.

Next we will talk about the main setup staring with //jwt setup

```csharp
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
```

This just simply get the section jwtSettings we created in appsettings.json earlier and store it in

```csharp
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
```

Here it gets the particular part of the section that is presented as key, and we encode it into bytes , the `!` tells the C# trust me bro it's not null;

**IMPORTANT -> cryptography cannot use strings so it is necessary to convert it into bytes**

Now the next step 

```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
```

Here it says that from now on my application will user jwtautheication as default mode of authenication and if authentication fails we will challenge the user using jwt rules , so it is now **jwt Sarkar** here
so it will give 401 unauthorized or redirects based on situations

Next thing 

```csharp
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
```

This tells the security guard what all you need to check before you can give entry , you need to validate issuer, audience and life time along with the key , the value of each is being given to you 
issuer and audience value is being taken directly from our jwtsettings which is the var that contains the the section of appsetthings.json with the same name

### Signing Keys

Now comes the line:
`IssuerSigningKey = new SymmetricSecurityKey(key)`

This **symmetricsecuritykey** is actually telling the jwt what type of key arcitethure we are using right now or what algorithm we are using or jwt should use to validate the key , as we are using **HS256** which is symmetric , like the smae key is getting use to sign in and verify which is apparently different from other arictethures 

**There is a assymetrickey sytem**
`new RsaSecurityKey(rsaPublicKey)`
that uses algorithm like RS256, RS384, RS512, this is used as a more secure option and is recommended when we are using more than one apis and authroization like OAuth , as this have different keys namely public and private key so even if one api got compromised not everyhting will get compromised alltogether and we use public key for the public apis so even if it get exposed it doesn't possess any risk us.

**Also there is a option called X.509 certificates**
`new X509SecurityKey(cert)`
Used when:
* keys rotate automatically
* trust chains matter
* you want OS / HSM support

Also the expiration time stamp is baked into jwt settings so bearer checks it with current time and know whether it is expired or not
.........................................................................................................
//next the swagger setup next

the goal -> To let us test protected API endpoints directly in the browser by pasting a JWT token once, instead of manually adding it to every request.

in other words we want to create a development friendly environment for testing our api , without these we need to use external tools like postman but with it we can directly send the jwt token for authorization and testing

// Swagger auth support

```
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {your token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
```

now this line 
```
builder.Services.AddSwaggerGen(c =>
{
    // Swagger config here
});
```
tells builder what settings to apply when swagger is being generated
next -> 
`c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme{...} ...)
this line just tells that hey i am going to define a security scheme with name "bearer" which is just a name/label

then we have 
`Name = "Authorization",
here we call it name as this represent the “The name of the HTTP header that will carry the token is Authorization.”
there are multiple http fields in the header 
a standard http request looks like 
GET /api/tickets HTTP/1.1
Host: example.com
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR...
so we use authorization here to send ours as this is a convention and other middleware like ,autheincation() and postman looks for it and recognises it 

`Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
says “This authentication method works using standard HTTP authentication.”

btw Swagger supports many authentication styles:
API keys
OAuth2
OpenID Connect
HTTP auth (Basic, Bearer, etc.)

Scheme = "bearer",
this tells the swagger that we are sending request using BEARER scheme

bearer -> Whoever bears (holds) this token is allowed access

Example formats:
Basic username:password
Digest ...
Bearer <token>

BearerFormat = "JWT",
The Bearer token format will be a JWT

`In = Microsoft.OpenApi.Models.ParameterLocation.Header,
tells swagger that token must be send via header not by body or url , as these are most easily logged and it is the start auth place and they work for all request like post get delete or put
request like get don't have a body and url get logged pretty easily

Description = "Enter JWT token like: Bearer {your token}"
just a standard description so user don't paste token without writing bearer

now 
```
c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
{
    {
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Reference = new Microsoft.OpenApi.Models.OpenApiReference
            {
                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }
});

```
This tells Swagger:“All API endpoints require the Bearer authentication scheme by default.”
here we are giving the id of what we created earlier and telling it to use that as the default , the name authorization and scheme bearer goes with it ,  

new string[] {//scopes here}
as in jwt we don;t use scopes we leave it empty , it is use by oauth2 , scopes include things like "read:users","write:tickets","admin" , just to say even if you are all valid but don't have the required rank you still can't access the things you are not allowed

todays rollercoaster- i tried to shift from .net 10 to .net 8 as i found out that 10 is still unstable for things and 8 is being used currently in the market so i have to deltetr everything and resdo also founf out that the code chatgpt was giving me was not of 10 anyway so the code more or less remains the smae add somthing simple in program cs add swagger but everything else is asem had to install new pacakges by eplixityl mentionng the versions name it was diffivult to get swagger working becuase of some confudino but it is working now 


Authcontroller now
- it handels login and registration requests from the user

```
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;//This line lets your controller talk to the database using Entity Framework.
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;//This line lets you create and write JWT tokens.
using System.Security.Claims;//This line lets you put user identity data inside the JWT token (like email, role, user ID).
using System.Text;
using SupportTicketSystem.Api.Data;
using SupportTicketSystem.Api.DTOs;
using SupportTicketSystem.Api.Helpers;
using SupportTicketSystem.Api.Models;
```

using Microsoft.AspNetCore.Mvc; this contains some helper tools for building a mvc controller system , here we will use it for things like [apicontroller] which tells our Asp.net that this controller is for an api and that helps in things like better error message easier http request handling such as ok(...), or unaithorized(...) bad request and such which is otherwise implementing frmo scratch is difficult

using Microsoft.IdentityModel.Tokens;
gives->SymmetricSecurityKey,
SigningCredentials,
Token signing tools
In simple terms:
This line lets you sign JWT tokens securely.

next->
[ApiController] tells your Asp.net that this is an api controller 
next->
[Route("api/[controller]")] this tells that all request handeled by this class will have this route and controller here will be replaced by the class name of ours which is authcontroller so this will become -> /api/auth

public class AuthController : ControllerBase
AuthController is just our class name and as its end with cotnroller asp.net automatically knows that it handles http requests
controllerbase means our class inherits funcitons form it that gives us things like ok(...), BadRequest(...),UnAuthorized(...)

private readonly ApplicationDbContext _context;
private readonly IConfiguration _config;

_context holds your database connection and _config holds your jwt secret issuer and other information
why private? becuase we just want to use them in this file and don;t want it to expose it through other means and 
why readonly? this means that their value will be given once in the constructor and cannot be change this prevents accidental replacement of them 

```
public AuthController(ApplicationDbContext context, IConfiguration config)
{
    _context = context;
    _config = config;
}
```
just a constructor that fills in the value for context and config
ApplicationDbContext context
IConfiguration config
ASP.NET automatically creates these objects and passes them in.
This is called Dependency Injection.
You do NOT manually create them. asp.net looks for exisitng one or create ones itself

next->

```
[HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists.");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                Role = dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

```
[HttpPost("register")] means this parts handels post rewuest send to api/auth/register
`public async Task<IActionResult> Register(RegisterDto dto);
here we put it as async so our server thread need to wait for hte rsult because it is interacting with the database and can take some time so async says that i will return some value but it may take some time don;t freeze the server , without this the mthos will keep on waiting for it to end and you can;t really use await
IActionResult this is an interface so our porgram can output any result define , these are http request outputs , like ok , badrequest and unauthorized
```
if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists.");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                Role = dto.Role
            };
```
here we wants to check if there is any user with the same email aready if yes then return bad request
else create a new user with the given information
the information is in the funciton registerdto dto , and is automatically taken by framework from the http request

Updated ApplicationDbContext
```
using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Api.Models;

namespace SupportTicketSystem.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<TicketComment> TicketComments => Set<TicketComment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.AssignedAgent)
                .WithMany()
                .HasForeignKey(t => t.AssignedAgentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketComment>()
                .HasOne(tc => tc.Ticket)
                .WithMany(t => t.Comments)
                .HasForeignKey(tc => tc.TicketId);

            modelBuilder.Entity<TicketComment>()
                .HasOne(tc => tc.User)
                .WithMany()
                .HasForeignKey(tc => tc.UserId);
        }
    }
}

```
here we are decalring three tables , users, ticket and ticketcomments
with the model builder we are actually decalring relation between the tables like froegin key and stuff
modelBuilder.Entity<Ticket>()
                .HasOne(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

here we are creating a column createdbyuser that can have only one that is one ticket may linked to only one user but one user can have multiple ticket , we are giving it id also and saying that don;t delet i twven if the user gets deleted
 something in sql language will look like
 ALTER TABLE Tickets
ADD CONSTRAINT FK_Tickets_Users_CreatedByUserId
FOREIGN KEY (CreatedByUserId)
REFERENCES Users(Id);



table will look like after agent and user 
Tickets
-------
Id (PK)
Title
Description
Status
Priority
CreatedByUserId (FK → Users.Id)//fk->foreign key
AssignedAgentId (FK → Users.Id, nullable)
CreatedAt
UpdatedAt
ResolvedAt (nullable)

why override? because we are inherting from dbcontext provided by ef which contains a method called onmodelcreating so we will override that method to define our schema.

after making other dtos you have to run
dotnet ef migrations add AddTickets
dotnet ef database update

this will sync your c# code structres of tabels to your sql server

```
using System.Security.Claims;

namespace SupportTicketSystem.Api.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
        public static string GetUserRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)!.Value;
        }
    }
}
```
here we are making a extension for claims principal class that is predefined in the library we are making two methods that returns us our userid and role of the person that is currently logged in , this is done by authorizing the token that comes with the request and verifying it then making the claims and use this method to access them.

`Priority = Enum.Parse<TicketPriority>(dto.Priority, true)
we parse this becuase the dto witht eh information coming from  is of type string as define in createticketdto but the priorty in the backend in model ticket.cs have it as a enum so we need to parse "high" as ticketpriority.high so we are parsing it here / also the true here means ignore the uppercase and lowercase

Iqueryable is used to write query we write query for sql then using 
await query.ToListAsync();
using this we send it to run in sql directly as this is better so we need to load all the data in our code and this can run in the database itself so it's more efficent 
later here 
```
  var ticket=await _context.Tickets
                .Include(t=>t.CreatedByUser)
                .Include(t=>t.AssignedAgent)
                .FirstOrDefaultAsync(t=>t.Id==id);
```
here we didn't used iqueryable becuase we already know what ticket to get and we need to get only one particular ticket
plus "You actually are using IQueryable internally.
_context.Tickets itself is an IQueryable<Ticket>.
But you are chaining everything and executing immediately with:
FirstOrDefaultAsync()
this Executes the SQL immediately."

we didn't write authorize becuase we need to check inside the method anyway plus we check the authorization before hand to stop it from entering the method to begin with and we need not to do that here

The HTTP PUT method is used to create a new resource or replace the entire representation of an existing resource at a specific, client-defined URL
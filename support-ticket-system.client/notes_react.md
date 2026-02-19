so lets move on to connecting our backend we have created in asp.net #8 to the frontend created using vite-not beta
in react and javascript

first we started with our api folder adding axios.js
```
import axios from "axios";

const instance = axios.create({
  baseURL: "https://localhost:5001/api",
});

instance.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default instance;

```

axios is a http client library for browsers and node.js that allows stuff like axios.get(),axios.post(),.put(),.delete etc. instead of using the build-in fetch API

why not fetch() that is buildin?
axios gives 
automatic json parsing :- axios automatically convers the resposne body from json to a JS object , so you don;t need to call .json() manually

automatic error rejection(for 4xx/5xx):-axios automatially throws an error when the server returns status code like 400 or 500 while you have to manually tell fetch that these are error or else it will treat them as normal resposnes

interceptors :-lets u automaticlaly modify every request ot response like attaching a jwt token before it reaches your code.(like a middle layer between the request and the recieve , intercepts the responses) more details below

cleaner syntax

global configuration:- lets u create a custom instance with shared settings like base URL and headers , so you don't repeat them in every request

so first we create a instance like telling instead of using hte global version i want to use my personal one wheer we define the base url so instead of using axios.get , we will now use instance.get().
profit? we can write
instance.get("/tickets"); instead of
axios.get("https://localhost:5001/api/tickets")
https://localhost:5001/api/tickets , this was define with our route in the controllers on backend

now 
`instance.interceptors.request.use((config) => {

as we know interceptor is a func that automatically runs before or after the request

two types of it
request interceptor - before the request goes to server
response interceptor- after response come back

.use() means register the function to run every time  here a request is made and then we define our config
so we call instance.get() , axios build request config this interceptor runs and modify the config with waht we gave to it then the request goes to the backend

then 
`const token = localStorage.getItem("token");

local storage here is a small storage in user's browser so we can store things there like a token or name and stuff that do not go away even after a refresh

```
if(token){
        config.headers.Authorization = `Bearer ${token}`;
    }
```
this line checks if the token exists and if it does 
it create a header like his 
` Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

//next the login.jsx
```
import { useState } from "react";
import axios from "../api/axios";
import { useNavigate } from "react-router-dom";

export default function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();

   const handleLogin = async (e) => {
    e.preventDefault();
    try {
      const res = await axios.post("/auth/login", {
        email,
        password,
      });

      localStorage.setItem("token", res.data.token);
      localStorage.setItem("role", res.data.role);

      navigate("/dashboard");
    } catch (err) {
      alert("Login failed");
    }
  };
  return (
    <form onSubmit={handleLogin}>
      <h2>Login</h2>
      <input
        placeholder="Email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
      />
      <input
        type="password"
        placeholder="Passwrod"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
      />
      <button type="submit">Login</button>
    </form>
  );
}
```
`const [email, setEmail] = useState("");
it means the inital value of the current value(email) is ""
so email=""
and when users sends setEmail("abc@gmail.com") email="abc@gmail.com"
const navigate = useNavigate();
this navigate is use to change the components of the page without refreshing using react dom this is how we build single page systems



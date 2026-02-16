using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Api.Data;
using SupportTicketSystem.Api.DTOs;
using SupportTicketSystem.Api.Helpers;
using SupportTicketSystem.Api.Models;

namespace SupportTicketSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TicketsController(ApplicationDbContext context)
        {
            _context=context;
        }
    

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketDto dto)
        {
            var userId = User.GetUserId();
            var ticket=new Ticket
            {
              Title=dto.Title,
              Description=dto.Description,
              Priority=Enum.Parse<TicketPriority>(dto.Priority,true),
              CreatedByUserId=userId  
            };
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            var userId=User.GetUserId();
            var role=User.GetUserRole();
            IQueryable<Ticket>query=_context.Tickets
                .Include(t=>t.CreatedByUser)
                .Include(t=>t.AssignedAgent);

            if(role == "Customer")
                query=query.Where(t=>t.CreatedByUserId==userId);
            else if(role=="Agent")
                query=query.Where(t=>t.AssignedAgentId==userId);
            var tickets=await query
            .OrderByDescending(t=>t.CreatedAt)
            .Select(t=>new TicketResponseDto
            {
                Id=t.Id,
                Title=t.Title,
                Description=t.Description,
                Status=t.Status.ToString(),
                Priority=t.Priority.ToString(),
                CreatedBy=t.CreatedByUser.FullName,
                AssignedAgent=t.AssignedAgent!=null?t.AssignedAgent.FullName:null,
                CreatedAt=t.CreatedAt,
                UpdatedAt=t.UpdatedAt


            })
            .ToListAsync();
            return Ok(tickets);

            
        }
        //view single ticket
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            var userId=User.GetUserId();
            var role=User.GetUserRole();
            var ticket=await _context.Tickets
                .Include(t=>t.CreatedByUser)
                .Include(t=>t.AssignedAgent)
                .FirstOrDefaultAsync(t=>t.Id==id);
            if(ticket==null)
                return NotFound();
            if(role=="Customer"&&ticket.CreatedByUserId!=userId)
                return Forbid();
            else if(role=="Agent"&&ticket.AssignedAgentId!=userId)
                return Forbid();
            
            return Ok(new TicketResponseDto
            {
                Id=ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status.ToString(),
                Priority = ticket.Priority.ToString(),
                CreatedBy = ticket.CreatedByUser.FullName,
                AssignedAgent = ticket.AssignedAgent?.FullName,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt
            });
        }
        //Agent or admin updates status
        [Authorize(Roles ="Agent,Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id,UpdateTicketStatusDto dto)
        {
            var ticket=await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();
            ticket.Status=Enum.Parse<TicketStatus>(dto.Status,true);
            ticket.UpdatedAt=DateTime.UtcNow;

            if (ticket.Status == TicketStatus.Resolved)
                ticket.ResolvedAt=DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok("Status Updated");
        }
        //Admin -> assign agent
        [Authorize(Roles ="Admin")]
        [HttpPut("{id}/assign")]
        public async Task<IActionResult> AssignAgent(int id,AssignTicketDto dto)
        {
            var ticket=await _context.Tickets.FindAsync(id);
            if(ticket==null)
                return NotFound();
            
            var agent=await _context.Users.FirstOrDefaultAsync(u=>u.Id==dto.AgentId && u.Role=="Agent");
            if(agent==null)
                return BadRequest("Invalid agent.");
            
            ticket.AssignedAgentId=agent.Id;
            ticket.UpdatedAt=DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok("Agent Assigned.");
        }
        //Admin: Delete Ticket
        [Authorize(Roles ="Admin")]
        [HttpDelete("{Id}")]

        public async Task<IActionResult> DeleteTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if(ticket==null)
                return NotFound();
            
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return Ok("Ticket Deleted");
        }
    
    }

}
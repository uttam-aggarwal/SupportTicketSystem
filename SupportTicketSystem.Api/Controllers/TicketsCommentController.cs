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
    [Route("api/tickets/{ticketid}/comments")]
    [Authorize]
    public class TicketCommentController:ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TicketCommentController(ApplicationDbContext context)
        {
            _context=context;
        }
        [HttpPost]
        public async Task<IActionResult> AddComment(int ticketId,CreateCommentDto dto)
        {
            var userId=User.GetUserId();
            var role=User.GetUserRole();

            var ticket=await _context.Tickets.FindAsync(ticketId);
            if(ticket==null)
                return NotFound();
            
            //Customer cannot add internal notes
            if(dto.IsInternal && role=="Customer")
                return Forbid();

            var comments=new TicketComment
            {
                TicketId=ticketId,
                UserId=userId,
                Message=dto.Message,
                IsInternal=dto.IsInternal
            };
            _context.TicketComments.Add(comments);
            await _context.SaveChangesAsync();
            return Ok("Comment added.");

        }

        [HttpPut]
        public async Task<IActionResult> GetComments(int ticketId)
        {
            var userId=User.GetUserId();
            var role=User.GetUserRole();

            var ticket=await _context.Tickets
            .Include(t=>t.Comments)
            .ThenInclude(c=>c.User)
            .FirstOrDefaultAsync(t=>t.Id==ticketId);

            if(ticket==null)
                return NotFound();
            
            //Authorization
            if(role=="Customer" && ticket.CreatedByUserId != userId)
                return Forbid();
            if(role=="Agent" && ticket.AssignedAgentId!=userId)
                return Forbid();
            
            var comments=ticket.Comments
            .Where(c=>!c.IsInternal||role!="Customer")
            .OrderBy(c=>c.CreatedAt)
            .Select(c => new
            {
                c.Id,
                c.Message,
                c.IsInternal,
                c.CreatedAt,
                User=c.User.FullName
            });
            return Ok(comments);
        }
    }
}
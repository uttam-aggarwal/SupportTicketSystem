namespace SupportTicketSystem.Api.Models
{
    public class Ticket
    {
        public int Id{get;set;}
        public string Title{get;set;}=null!;
        public string description{get;set;}=null!;
        public TicketStatus status{get;set;}=TicketStatus.Open;
        public TicketPriority Priority{get;set;}=TicketPriority.Medium;

        public int CreatedByUserId{get;set;}
        public User CreatedByUser{get;set;}=null!;

        public int? AssignedAgentId{get;set;}
        public User? AssignedAgentUser{get;set;}

        public DateTime CreatedAt{get;set;}=DateTime.UtcNow;
        public DateTime UpdatedAT{get;set;}=DateTime.UtcNow;
        public DateTime? ResolvedAt{get;set;}=DateTime.UtcNow;

        public ICollection<TicketComment> Comments{get;set;}=new List<TicketComment>();

    }
    public enum TicketStatus
    {
        Open,
        Inprogress,
        WaitingOnCustomer,
        Resolved,
        Closed
    }
    public enum TicketPriority
    {
        Low,
        Medium,
        High,
        Critical
    }
}
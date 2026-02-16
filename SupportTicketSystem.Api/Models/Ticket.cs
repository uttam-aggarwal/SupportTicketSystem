namespace SupportTicketSystem.Api.Models
{
    public class Ticket
    {
        public int Id{get;set;}
        public string Title{get;set;}=null!;
        public string Description{get;set;}=null!;
        public TicketStatus Status{get;set;}=TicketStatus.Open;
        public TicketPriority Priority{get;set;}=TicketPriority.Medium;

        public int CreatedByUserId{get;set;}
        public User CreatedByUser{get;set;}=null!;

        public int? AssignedAgentId{get;set;}
        public User? AssignedAgent{get;set;}

        public DateTime CreatedAt{get;set;}=DateTime.UtcNow;
        public DateTime UpdatedAt{get;set;}=DateTime.UtcNow;
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
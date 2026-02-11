namespace SupportTicketSystem.Api.DTOs
{
    public class TicketResponseDto
    {
        public int Id{get;set;}
        public string Title{get;set;}=null!;
        public string Description{get;set;}=null!;
        public string Status{get;set;}=null!;
        public string Priority{get;set;}=null!;
        public string CreatedBy{get;set;}=null!;
        public string? AssignedAgent{get;set;}=null!;

        public DateTime CreatedAt{get;set;}
        public DateTime UpdatedAt{get;set;}


    }
}
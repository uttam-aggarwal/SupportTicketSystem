namespace SupportTicketSystem.Api.DTOs
{
    public class CreateTicketDto
    {
        public string Title{get;set;}=null!;
        public string Description{get;set;}=null!;
        public string Priority{get;set;}="Medium";
    }
}
namespace SupportTicketSystem.Api.DTOs
{
    public class CreateCommentDto
    {
        public String Message{get;set;}=null!;
        public bool IsInternal{get;set;}=false;
    }
}
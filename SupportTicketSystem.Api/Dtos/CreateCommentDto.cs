namespace SupportTicketSystem.Api.DTOs
{
    public class CreateCommentDto
    {
        public String message{get;set;}=null!;
        public bool isInternal{get;set;}=false;
    }
}
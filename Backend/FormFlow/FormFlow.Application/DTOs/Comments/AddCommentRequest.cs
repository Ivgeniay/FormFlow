namespace FormFlow.Application.DTOs.Comments
{
    public class AddCommentRequest
    {
        public Guid TemplateId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}

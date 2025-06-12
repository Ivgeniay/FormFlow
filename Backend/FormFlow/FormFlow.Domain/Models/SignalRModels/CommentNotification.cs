namespace FormFlow.Domain.Models.SignalRModels
{
    public class CommentNotification
    {
        public Guid Id { get; set; }
        public Guid TemplateId { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

}

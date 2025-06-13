using FormFlow.Application.DTOs.Comments;

namespace FormFlow.Infrastructure.Models.SignalREventModels
{
    public class NewCommentEvent
    {
        public CommentDto Comment { get; set; } = new CommentDto();
        public Guid TemplateId { get; set; }
        public DateTime AddedAt { get; set; }
    }
}

using FormFlow.Application.DTOs.Comments;

namespace FormFlow.Infrastructure.Models.SignalREventModels
{
    public class CommentAddedEvent
    {
        public bool Success { get; set; }
        public CommentDto Comment { get; set; } = new CommentDto();
    }
}

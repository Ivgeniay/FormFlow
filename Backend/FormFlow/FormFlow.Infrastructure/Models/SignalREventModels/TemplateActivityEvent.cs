using FormFlow.Application.DTOs.Comments;

namespace FormFlow.Infrastructure.Models.SignalREventModels
{
    public class TemplateActivityEvent
    {
        public Guid TemplateId { get; set; }
        public List<CommentDto> RecentComments { get; set; } = new List<CommentDto>();
        public int LikesCount { get; set; }
        public bool UserLiked { get; set; }
        public DateTime LoadedAt { get; set; }
    }
}

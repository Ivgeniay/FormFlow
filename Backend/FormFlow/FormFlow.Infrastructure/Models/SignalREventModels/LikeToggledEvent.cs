namespace FormFlow.Infrastructure.Models.SignalREventModels
{
    public class LikeToggledEvent
    {
        public Guid TemplateId { get; set; }
        public int TotalLikes { get; set; }
        public bool IsLiked { get; set; }
        public string Action { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}

namespace FormFlow.Domain.Models.SignalRModels
{
    public class LikeUpdateNotification
    {
        public Guid TemplateId { get; set; }
        public int TotalLikes { get; set; }
        public bool UserLiked { get; set; }
        public Guid? LastLikeUserId { get; set; }
        public string? LastLikeUserName { get; set; }
    }

}

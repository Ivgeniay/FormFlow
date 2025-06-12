namespace FormFlow.Application.DTOs.Likes
{
    public class LikeNotificationDto
    {
        public Guid TemplateId { get; set; }
        public int TotalLikes { get; set; }
        public bool UserLiked { get; set; }
        public string? LastLikeUserName { get; set; }
    }
}

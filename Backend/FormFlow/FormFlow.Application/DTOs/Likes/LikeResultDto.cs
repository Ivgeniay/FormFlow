namespace FormFlow.Application.DTOs.Likes
{
    public class LikeResultDto
    {
        public bool IsLiked { get; set; }
        public int TotalLikes { get; set; }
        // added/remoded
        public string Action { get; set; } = string.Empty;
        public Guid? LastLikeUserId { get; set; }
        public string? LastLikeUserName { get; set; }
    }
}

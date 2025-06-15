namespace FormFlow.Domain.Models.Analytics
{
    public class UserAnalyticsStatsDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TemplatesCount { get; set; }
        public int FormsCount { get; set; }
        public int CommentsCount { get; set; }
        public int LikesGivenCount { get; set; }
    }
}

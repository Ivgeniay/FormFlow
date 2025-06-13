namespace FormFlow.Infrastructure.Models.SignalREventModels
{
    public class UserLeftEvent
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid TemplateId { get; set; }
        public DateTime LeftAt { get; set; }
    }
}

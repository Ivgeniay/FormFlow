namespace FormFlow.Infrastructure.Models.SignalREventModels
{
    public class UserJoinedEvent
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid TemplateId { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}

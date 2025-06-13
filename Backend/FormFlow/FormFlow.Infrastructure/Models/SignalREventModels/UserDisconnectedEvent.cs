namespace FormFlow.Infrastructure.Models.SignalREventModels
{
    public class UserDisconnectedEvent
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime DisconnectedAt { get; set; }
    }
}

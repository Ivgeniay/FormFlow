namespace FormFlow.Infrastructure.Models.SignalREventModels
{
    public class ConnectedEvent
    {
        public string ConnectionId { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public bool IsAuthenticated { get; set; }
        public DateTime ConnectedAt { get; set; }
    }
}

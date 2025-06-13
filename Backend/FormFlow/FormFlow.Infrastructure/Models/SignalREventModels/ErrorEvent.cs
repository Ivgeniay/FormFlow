namespace FormFlow.Infrastructure.Models.SignalREventModels
{
    public class ErrorEvent
    {
        public string Message { get; set; } = string.Empty;
        public string? ErrorCode { get; set; }
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}

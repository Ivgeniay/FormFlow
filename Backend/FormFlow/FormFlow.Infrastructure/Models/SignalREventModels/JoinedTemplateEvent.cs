namespace FormFlow.Infrastructure.Models.SignalREventModels
{
    public class JoinedTemplateEvent
    {
        public Guid TemplateId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

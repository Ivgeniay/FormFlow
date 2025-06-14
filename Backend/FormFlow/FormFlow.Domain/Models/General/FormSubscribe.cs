namespace FormFlow.Domain.Models.General
{
    public class FormSubscribe
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid TemplateId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

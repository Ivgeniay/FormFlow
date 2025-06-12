namespace FormFlow.Domain.Models.General
{
    public class TemplateTag
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TemplateId { get; set; }
        public Guid TagId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Template Template { get; set; }
        public Tag Tag { get; set; }
    }
}

namespace FormFlow.Domain.Models.General
{
    public class Like
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TemplateId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Template Template { get; set; }
        public User User { get; set; }
    }
}

namespace FormFlow.Application.DTOs.Tags
{
    public class TagDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int UsageCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

namespace FormFlow.Application.DTOs.Tags
{
    public class TagCloudItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int UsageCount { get; set; }
        public int Weight { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

namespace FormFlow.Application.DTOs.Tags
{
    public class CloudTagDto
    {
        public List<TagCloudItemDto> Tags { get; set; } = new List<TagCloudItemDto>();
        public int MaxUsageCount { get; set; }
        public int MinUsageCount { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}

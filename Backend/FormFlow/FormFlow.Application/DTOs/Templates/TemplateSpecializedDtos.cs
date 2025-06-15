namespace FormFlow.Application.DTOs.Templates
{
    public class TemplateListItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid TopicId { get; set; }
        public string? ImageUrl { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public int FormsCount { get; set; }
        public int LikesCount { get; set; }
    }

    public class TemplateSearchDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
    }
}

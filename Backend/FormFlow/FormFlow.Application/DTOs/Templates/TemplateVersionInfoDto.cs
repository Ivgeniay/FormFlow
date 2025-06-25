namespace FormFlow.Application.DTOs.Templates
{
    public class TemplateVersionInfoDto
    {
        public Guid BaseTemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public int TotalVersions { get; set; }
        public DateTime FirstCreated { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<TemplateVersionSummaryDto> Versions { get; set; } = new List<TemplateVersionSummaryDto>();
    }

    public class TemplateVersionSummaryDto
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public bool IsArchived { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int FormsCount { get; set; }
        public int QuestionsCount { get; set; }
    }
}

using FormFlow.Domain.Models.General;

namespace FormFlow.Application.DTOs.Templates
{
    public class CreateTemplateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid TopicId { get; set; }
        public TemplateAccess AccessType { get; set; } = TemplateAccess.Public;
        public List<string> Tags { get; set; } = new List<string>();
        public List<Guid> AllowedUserIds { get; set; } = new List<Guid>();
    }

    public class UpdateTemplateRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid TopicId { get; set; }
        public TemplateAccess AccessType { get; set; }
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
        public List<string> Tags { get; set; } = new List<string>();
        public List<Guid> AllowedUserIds { get; set; } = new List<Guid>();
    }

    public class UpdateTemplateTagsRequest
    {
        public List<string> Tags { get; set; } = new List<string>();
    }
}

using System.Dynamic;

namespace FormFlow.Domain.Interfaces.Services
{
    public interface IAiTemplateService
    {
        Task<AiTemplateDto> GenerateFromPromptAsync(string prompt);
    }

    public class AiTemplateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public List<string> SuggestedTags { get; set; } = new List<string>();
        public List<AiQuestionsDto> Questions { get; set; } = new List<AiQuestionsDto>();
    }

    public class AiQuestionsDto
    {
        public int Order { get; set; } = 1;
        public bool ShowInResults { get; set; } 
        public bool IsRequired { get; set; }
        public dynamic Data { get; set; } = new ExpandoObject();
    }
}

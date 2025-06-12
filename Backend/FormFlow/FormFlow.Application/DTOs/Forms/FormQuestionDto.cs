using FormFlow.Domain.Models.General;

namespace FormFlow.Application.DTOs.Forms
{
    public class FormQuestionDto
    {
        public Guid QuestionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public int Order { get; set; }
        public bool IsRequired { get; set; }
        public string QuestionData { get; set; } = string.Empty;
        public object? Answer { get; set; }
    }
}

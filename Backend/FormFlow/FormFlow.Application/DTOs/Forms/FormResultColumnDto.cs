using FormFlow.Domain.Models.General;

namespace FormFlow.Application.DTOs.Forms
{
    public class FormResultColumnDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionTitle { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public string DisplayValue { get; set; } = string.Empty;
    }
}

namespace FormFlow.Application.DTOs.Forms
{
    public class FormDto
    {
        public Guid Id { get; set; }
        public Guid TemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<FormQuestionDto> Questions { get; set; } = new();
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}

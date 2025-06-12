namespace FormFlow.Application.DTOs.Forms
{
    public class FormListItemDto
    {
        public Guid Id { get; set; }
        public Guid TemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public List<FormResultColumnDto> DisplayColumns { get; set; } = new();
    }
}

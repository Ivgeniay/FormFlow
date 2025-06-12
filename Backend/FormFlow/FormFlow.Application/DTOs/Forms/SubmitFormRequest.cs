namespace FormFlow.Application.DTOs.Forms
{
    public class SubmitFormRequest
    {
        public Guid TemplateId { get; set; }
        public Dictionary<Guid, object> Answers { get; set; } = new();
        public bool SendCopyToEmail { get; set; } = false;
    }
}

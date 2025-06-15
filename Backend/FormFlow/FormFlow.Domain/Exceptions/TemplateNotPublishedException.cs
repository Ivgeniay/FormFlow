namespace FormFlow.Application.Services
{
    public class TemplateNotPublishedException : Exception
    {
        public TemplateNotPublishedException(Guid templateId) : base($"Template with ID '{templateId}' is not published.")
        {
        }
    }
}

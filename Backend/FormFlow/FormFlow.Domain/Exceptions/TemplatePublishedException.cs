namespace FormFlow.Application.Services
{
    public class TemplatePublishedException : Exception
    {
        public TemplatePublishedException(Guid templateId) : base($"Template with ID '{templateId}' is published and cannot be modified.")
        {
        }
    }
}

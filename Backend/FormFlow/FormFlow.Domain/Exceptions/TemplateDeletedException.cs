namespace FormFlow.Application.Services
{
    public class TemplateDeletedException : Exception
    {
        public TemplateDeletedException(Guid templateId) : base($"Template with ID '{templateId}' has been deleted.")
        {
        }
    }
}

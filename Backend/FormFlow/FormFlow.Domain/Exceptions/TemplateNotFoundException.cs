namespace FormFlow.Domain.Exceptions
{
    public class TemplateNotFoundException : DomainException
    {
        public TemplateNotFoundException(Guid templateId)
            : base($"Template with ID '{templateId}' was not found.") { }

    }
}

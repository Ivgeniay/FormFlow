namespace FormFlow.Domain.Exceptions
{
    public class TemplateAccessDeniedException : DomainException
    {
        public TemplateAccessDeniedException(Guid templateId, Guid userId)
            : base($"User '{userId}' does not have access to template '{templateId}'.") { }
    }
}

namespace FormFlow.Domain.Exceptions
{
    public class FormAlreadySubmittedException : DomainException
    {
        public FormAlreadySubmittedException(Guid templateId, Guid userId)
            : base($"User '{userId}' has already submitted a form for template '{templateId}'.") { }
    }
}

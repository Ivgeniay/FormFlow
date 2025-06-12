namespace FormFlow.Domain.Exceptions
{
    public class FormNotFoundException : DomainException
    {
        public FormNotFoundException(Guid formId)
            : base($"Form with ID '{formId}' was not found.") { }
    }
}

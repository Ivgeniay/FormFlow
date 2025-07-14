namespace FormFlow.Domain.Exceptions
{
    public class ApiTokenAccessDeniedException : DomainException
    {
        public ApiTokenAccessDeniedException()
            : base("Access denied for API token operation.") { }

        public ApiTokenAccessDeniedException(string message)
            : base(message) { }
    }
}
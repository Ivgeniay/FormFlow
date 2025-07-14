namespace FormFlow.Domain.Exceptions
{
    public class ApiTokenNotFoundException : DomainException
    {
        public ApiTokenNotFoundException(Guid tokenId)
            : base($"API token with ID '{tokenId}' was not found.") { }

        public ApiTokenNotFoundException(string token)
            : base($"API token '{token}' was not found.") { }
    }
}
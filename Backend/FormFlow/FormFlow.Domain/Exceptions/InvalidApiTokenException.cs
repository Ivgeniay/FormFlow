namespace FormFlow.Domain.Exceptions
{
    public class InvalidApiTokenException : DomainException
    {
        public InvalidApiTokenException()
            : base("Invalid or inactive API token.") { }

        public InvalidApiTokenException(string message)
            : base(message) { }
    }
}
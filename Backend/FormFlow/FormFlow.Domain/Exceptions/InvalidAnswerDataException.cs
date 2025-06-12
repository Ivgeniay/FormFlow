namespace FormFlow.Domain.Exceptions
{
    public class InvalidAnswerDataException : DomainException
    {
        public InvalidAnswerDataException(string reason)
            : base($"Invalid answer data: {reason}") { }
    }
}

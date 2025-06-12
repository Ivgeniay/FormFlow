namespace FormFlow.Domain.Exceptions
{
    public class InvalidQuestionDataException : DomainException
    {
        public InvalidQuestionDataException(string reason)
            : base($"Invalid question data: {reason}") { }
    }
}

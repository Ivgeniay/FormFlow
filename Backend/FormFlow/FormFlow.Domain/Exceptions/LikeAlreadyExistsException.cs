namespace FormFlow.Domain.Exceptions
{
    public class LikeAlreadyExistsException : DomainException
    {
        public LikeAlreadyExistsException(Guid templateId, Guid userId)
            : base($"User '{userId}' has already liked template '{templateId}'.") { }
    }
}

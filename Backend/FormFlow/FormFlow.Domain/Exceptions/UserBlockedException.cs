namespace FormFlow.Domain.Exceptions
{
    public class UserBlockedException : DomainException
    {
        public UserBlockedException(Guid userId)
            : base($"User with ID '{userId}' is blocked and cannot perform this action.") { }
    }
}

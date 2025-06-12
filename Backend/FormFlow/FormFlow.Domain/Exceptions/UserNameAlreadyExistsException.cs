namespace FormFlow.Domain.Exceptions
{
    public class UserNameAlreadyExistsException : DomainException
    {
        public UserNameAlreadyExistsException(string userName)
            : base($"User with username '{userName}' already exists.") { }
    }
}

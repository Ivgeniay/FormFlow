namespace FormFlow.Domain.Exceptions
{
    public class CommentNotFoundException : DomainException
    {
        public CommentNotFoundException(Guid commentId)
            : base($"Comment with ID '{commentId}' was not found.") { }
    }
}

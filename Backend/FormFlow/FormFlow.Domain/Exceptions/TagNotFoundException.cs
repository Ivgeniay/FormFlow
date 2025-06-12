namespace FormFlow.Domain.Exceptions
{
    public class TagNotFoundException : DomainException
    {
        public TagNotFoundException(Guid tagId)
            : base($"Tag with ID '{tagId}' was not found.") { }

        public TagNotFoundException(string tagName)
            : base($"Tag with name '{tagName}' was not found.") { }
    }
}

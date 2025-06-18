namespace FormFlow.Domain.Models.General
{
    public class Topic
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public List<Template> Templates { get; set; }
    }
}

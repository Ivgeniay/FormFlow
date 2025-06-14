namespace FormFlow.Domain.Models.General
{
    public class Question
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TemplateId { get; set; }
        public int Order { get; set; }
        public bool ShowInResults { get; set; } = false;
        public bool IsRequired { get; set; } = false;
        public string Data { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        public Template Template { get; set; }
    }

    public enum QuestionType
    {
        ShortText = 1,
        LongText = 2,
        SingleChoice = 3,
        MultipleChoice = 4,
        Dropdown = 5,
        Scale = 6,
        Rating = 7,
        Date = 8,
        Time = 9
    }
}

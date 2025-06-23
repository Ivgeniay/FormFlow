namespace FormFlow.Domain.Models.General.QuestionDetailsModels
{
    public abstract class QuestionDetails
    {
        public int Id { get; set; }
        public QuestionType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsRequired { get; set; }

    }
}

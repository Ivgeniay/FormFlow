namespace FormFlow.Domain.Models.General.QuestionDetailsModels
{
    public abstract class QuestionDetails
    {
        public QuestionType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}

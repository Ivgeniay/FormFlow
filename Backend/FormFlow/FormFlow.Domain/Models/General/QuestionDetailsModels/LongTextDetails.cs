namespace FormFlow.Domain.Models.General.QuestionDetailsModels
{
    public class LongTextDetails : QuestionDetails
    {
        public int? MaxLength { get; set; }
        public string? Placeholder { get; set; }
    }
}

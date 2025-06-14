namespace FormFlow.Domain.Models.General.QuestionDetailsModels
{
    public class DateDetails : QuestionDetails
    {
        public DateTime Date { get; set; }
        public bool IncludeTime { get; set; } = false;
    }
}

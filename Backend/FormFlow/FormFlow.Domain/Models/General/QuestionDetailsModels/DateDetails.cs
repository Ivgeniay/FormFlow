namespace FormFlow.Domain.Models.General.QuestionDetailsModels
{
    public class DateDetails : QuestionDetails
    {
        public DateTime StartDate { get; set; }
        public DateTime PastDate { get; set; }
    }

    public class TimeDetails : QuestionDetails
    {
        public DateTime StartDate { get; set; }
        public DateTime PastDate { get; set; }
    }
}

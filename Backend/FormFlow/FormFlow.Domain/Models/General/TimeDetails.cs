using FormFlow.Domain.Models.General.QuestionDetailsModels;

namespace FormFlow.Domain.Models.General
{
    public class TimeDetails : QuestionDetails
    {
        public bool Use24HourFormat { get; set; } = true;
        public double UTCOffset { get; set; } = 0.0;
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
    }
}

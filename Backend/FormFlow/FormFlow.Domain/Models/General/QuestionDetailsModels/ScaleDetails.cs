namespace FormFlow.Domain.Models.General.QuestionDetailsModels
{
    public class ScaleDetails : QuestionDetails
    {
        public int MinValue { get; set; } = 0;
        public int MaxValue { get; set; } = 5;
        public string? MinLabel { get; set; }
        public string? MaxLabel { get; set; }
    }
}

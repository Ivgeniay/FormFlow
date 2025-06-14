namespace FormFlow.Domain.Models.General.QuestionDetailsModels
{
    public class RatingDetails : QuestionDetails
    {
        public int MaxRating { get; set; } = 5;
        public string? RatingLabel { get; set; }
    }
}

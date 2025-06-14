namespace FormFlow.Domain.Models.General.QuestionDetailsModels
{
    public class SingleChoiceDetails : QuestionDetails
    {
        public List<string> Options { get; set; } = new List<string>();
    }
}

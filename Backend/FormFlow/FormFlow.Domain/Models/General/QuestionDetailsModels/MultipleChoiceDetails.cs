namespace FormFlow.Domain.Models.General.QuestionDetailsModels
{
    public class MultipleChoiceDetails : QuestionDetails
    {
        public List<string> Options { get; set; } = new List<string>();
        public int? MaxSelections { get; set; }
        public int? MinSelections { get; set; }
    }
}

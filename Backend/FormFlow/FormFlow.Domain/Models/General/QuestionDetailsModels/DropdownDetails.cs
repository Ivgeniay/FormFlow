namespace FormFlow.Domain.Models.General.QuestionDetailsModels
{
    public class DropdownDetails : QuestionDetails
    {
        public List<string> Options { get; set; } = new List<string>();
        public string? DefaultOption { get; set; }
    }
}

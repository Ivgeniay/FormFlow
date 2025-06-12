namespace FormFlow.Domain.Models.General
{
    public class Form
    {
        public Guid Id { get; set; }
        public Guid TemplateId { get; set; }
        public Guid UserId { get; set; }
        public string AnswersData { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public int TemplateVersion { get; set; }

        public Template Template { get; set; }
        public User User { get; set; }
    }

    public class FormAnswers
    {
        public Dictionary<Guid, AnswerDetails> Answers { get; set; } = new();
    }

    public abstract class AnswerDetails
    {
        public QuestionType Type { get; set; }
    }

    public class ShortTextAnswer : AnswerDetails
    {
        public string Value { get; set; } = string.Empty;
    }

    public class LongTextAnswer : AnswerDetails
    {
        public string Value { get; set; } = string.Empty;
    }

    public class SingleChoiceAnswer : AnswerDetails
    {
        public int SelectedIndex { get; set; }
        public string SelectedValue { get; set; } = string.Empty;
    }

    public class MultipleChoiceAnswer : AnswerDetails
    {
        public List<int> SelectedIndexes { get; set; } = new List<int>();
        public List<string> SelectedValues { get; set; } = new List<string>();
    }

    public class ScaleAnswer : AnswerDetails
    {
        public int Value { get; set; }
    }

    public class DateAnswer : AnswerDetails
    {
        public DateTime Value { get; set; }
    }
}

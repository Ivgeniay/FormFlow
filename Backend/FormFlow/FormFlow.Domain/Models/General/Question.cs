namespace FormFlow.Domain.Models.General
{
    public class Question
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TemplateId { get; set; }
        public int Order { get; set; }
        public bool ShowInResults { get; set; } = false;
        public bool IsRequired { get; set; } = false;
        public string Data { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        public Template Template { get; set; }
    }



    public enum QuestionType
    {
        ShortText = 1,
        LongText = 2,
        SingleChoice = 3,
        MultipleChoice = 4,
        Dropdown = 5,
        Scale = 6,
        Rating = 7,
        Date = 8,
        Time = 9
    }
    public abstract class QuestionDetails
    {
        public QuestionType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class ShortTextDetails : QuestionDetails
    {
        public int? MaxLength { get; set; }
        public string? Placeholder { get; set; }
    }

    public class LongTextDetails : QuestionDetails
    {
        public int? MaxLength { get; set; }
        public string? Placeholder { get; set; }
    }

    public class SingleChoiceDetails : QuestionDetails
    {
        public List<string> Options { get; set; } = new List<string>();
    }

    public class MultipleChoiceDetails : QuestionDetails
    {
        public List<string> Options { get; set; } = new List<string>();
        public int? MaxSelections { get; set; }
        public int? MinSelections { get; set; }
    }

    public class DropdownDetails : QuestionDetails
    {
        public List<string> Options { get; set; } = new List<string>();
        public string? DefaultOption { get; set; }
    }

    public class ScaleDetails : QuestionDetails
    {
        public int MinValue { get; set; } = 0;
        public int MaxValue { get; set; } = 5;
        public string? MinLabel { get; set; }
        public string? MaxLabel { get; set; }
    }

    public class RatingDetails : QuestionDetails
    {
        public int MaxRating { get; set; } = 5;
        public string? RatingLabel { get; set; }
    }

    public class DateDetails : QuestionDetails
    {
        public DateTime Date { get; set; }
        public bool IncludeTime { get; set; } = false;
    }

    public class TimeDetails : QuestionDetails
    {
        public bool Use24HourFormat { get; set; } = true;
        public double UTCOffset { get; set; } = 0.0;
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
    }
}

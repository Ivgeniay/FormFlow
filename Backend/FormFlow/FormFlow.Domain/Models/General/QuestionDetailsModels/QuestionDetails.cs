using System.Text.Json.Serialization;

namespace FormFlow.Domain.Models.General.QuestionDetailsModels
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(ShortTextDetails), "shortText")]
    [JsonDerivedType(typeof(LongTextDetails), "longText")]
    [JsonDerivedType(typeof(SingleChoiceDetails), "singleChoice")]
    [JsonDerivedType(typeof(MultipleChoiceDetails), "multipleChoice")]
    [JsonDerivedType(typeof(DropdownDetails), "dropdown")]
    [JsonDerivedType(typeof(ScaleDetails), "scale")]
    [JsonDerivedType(typeof(RatingDetails), "rating")]
    [JsonDerivedType(typeof(DateDetails), "date")]
    [JsonDerivedType(typeof(TimeDetails), "time")]
    public abstract class QuestionDetails
    {
        public int Id { get; set; }
        public QuestionType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsRequired { get; set; }

    }
}

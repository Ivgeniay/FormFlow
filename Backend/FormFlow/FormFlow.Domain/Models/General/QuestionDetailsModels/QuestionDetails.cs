using Newtonsoft.Json;

namespace FormFlow.Domain.Models.General.QuestionDetailsModels
{
    public class QuestionDetails
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        public QuestionType Type { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;
        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;
        [JsonProperty("isrequired")]
        public bool IsRequired { get; set; }
    }
}

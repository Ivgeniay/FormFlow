namespace FormFlow.Application.DTOs.Templates
{
    public class QuestionDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public bool ShowInResults { get; set; }
        public bool IsRequired { get; set; }
        public string Data { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

}

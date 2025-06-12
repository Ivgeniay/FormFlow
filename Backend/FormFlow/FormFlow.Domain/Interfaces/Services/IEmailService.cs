namespace FormFlow.Domain.Interfaces.Services
{
    public interface IEmailService
    {
        Task<bool> SendFormAnswersAsync(string toEmail, FormEmailData formData);
        Task<bool> SendEmailAsync(string toEmail, EmailTemplate message);
        Task<bool> IsAvailableAsync();
    }

    public class FormEmailData
    {
        public string TemplateName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public List<FormAnswerEmailItem> Answers { get; set; } = new List<FormAnswerEmailItem>();
    }

    public class FormAnswerEmailItem
    {
        public string QuestionTitle { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }

    public class EmailTemplate
    {
        public string Subject { get; set; } = string.Empty;
        public string HtmlBody { get; set; } = string.Empty;
        public string TextBody { get; set; } = string.Empty;
    }
}

namespace FormFlow.Domain.Interfaces.Services
{
    public interface IEmailService
    {
        Task<bool> SendFormAnswersAsync(string toEmail, Guid formId);
        Task<bool> SendEmailAsync(EmailTemplate message);
    }

    public class EmailTemplate
    {
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string HtmlBody { get; set; } = string.Empty;
        public string TextBody { get; set; } = string.Empty;
    }
}

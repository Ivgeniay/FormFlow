using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Interfaces.Services;
using FormFlow.Domain.Models.General;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.Json;
using FormFlow.Domain.Models.General.QuestionDetailsModels;

namespace FormFlow.Infrastructure.Services
{
    public class GmailEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IFormRepository _formRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ILogger<GmailEmailService> _logger;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _login;
        private readonly string _password;

        public GmailEmailService(
            IConfiguration configuration,
            IFormRepository formRepository,
            ITemplateRepository templateRepository,
            ILogger<GmailEmailService> logger)
        {
            _configuration = configuration;
            _formRepository = formRepository;
            _templateRepository = templateRepository;
            _logger = logger;

            _smtpServer = _configuration["GmailSMTP:Server"] ?? throw new ArgumentNullException("GmailSMTP:Server");
            _smtpPort = int.Parse(_configuration["GmailSMTP:Port"] ?? "587");
            _login = _configuration["GmailSMTP:Login"] ?? throw new ArgumentNullException("GmailSMTP:Login");
            _password = _configuration["GmailSMTP:Password"] ?? throw new ArgumentNullException("GmailSMTP:Password");
        }

        public async Task<bool> SendEmailAsync(EmailTemplate message)
        {
            try
            {
                using var client = CreateSmtpClient();
                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_login, "FormFlow"),
                    Subject = message.Subject,
                    Body = message.HtmlBody,
                    IsBodyHtml = true
                };

                var toAddresses = message.ToEmail.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var address in toAddresses)
                {
                    mailMessage.To.Add(address.Trim());
                }

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {ToEmail}", message.ToEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToEmail}: {Message}", message.ToEmail, ex.Message);
                return false;
            }
        }

        public async Task<bool> SendFormAnswersAsync(string toEmail, Guid formId)
        {
            try
            {
                var form = await _formRepository.GetWithAllDetailsAsync(formId);
                if (form == null)
                {
                    _logger.LogWarning("Form not found: {FormId}", formId);
                    return false;
                }

                var template = await _templateRepository.GetWithQuestionsAsync(form.TemplateId);
                if (template == null)
                {
                    _logger.LogWarning("Template not found: {TemplateId}", form.TemplateId);
                    return false;
                }

                var htmlBody = await GenerateFormAnswersHtmlAsync(form, template);
                var emailTemplate = new EmailTemplate
                {
                    ToEmail = toEmail,
                    Subject = $"Ответы на форму: {template.Title}",
                    HtmlBody = htmlBody,
                    TextBody = string.Empty
                };

                return await SendEmailAsync(emailTemplate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send form answers to {ToEmail} for form {FormId}: {Message}", toEmail, formId, ex.Message);
                return false;
            }
        }

        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_login, _password),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
        }

        private async Task<string> GenerateFormAnswersHtmlAsync(Form form, Template template)
        {
            var answers = JsonSerializer.Deserialize<Dictionary<Guid, object>>(form.AnswersData) ?? new Dictionary<Guid, object>();
            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html><head><meta charset='utf-8'>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; color: #333; }");
            html.AppendLine(".header { background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin-bottom: 20px; }");
            html.AppendLine(".question { margin-bottom: 15px; padding: 15px; border-left: 4px solid #007bff; background-color: #f8f9fa; }");
            html.AppendLine(".question-title { font-weight: bold; color: #495057; margin-bottom: 5px; }");
            html.AppendLine(".answer { color: #212529; }");
            html.AppendLine(".footer { margin-top: 30px; padding: 15px; background-color: #e9ecef; border-radius: 8px; font-size: 14px; color: #6c757d; }");
            html.AppendLine("</style></head><body>");

            html.AppendLine("<div class='header'>");
            html.AppendLine($"<h2>Ответы на форму: {template.Title}</h2>");
            html.AppendLine($"<p><strong>Описание:</strong> {template.Description}</p>");
            html.AppendLine($"<p><strong>Автор:</strong> {template.Author?.UserName}</p>");
            html.AppendLine($"<p><strong>Отправлено:</strong> {form.SubmittedAt:dd.MM.yyyy HH:mm}</p>");
            html.AppendLine($"<p><strong>Пользователь:</strong> {form.User?.UserName}</p>");
            html.AppendLine("</div>");

            foreach (var question in template.Questions.Where(q => !q.IsDeleted).OrderBy(q => q.Order))
            {
                var questionDetails = JsonSerializer.Deserialize<QuestionDetails>(question.Data);
                var answerValue = answers.ContainsKey(question.Id) ? answers[question.Id] : null;
                var displayAnswer = FormatAnswerForEmail(answerValue, questionDetails?.Type ?? QuestionType.ShortText);

                html.AppendLine("<div class='question'>");
                html.AppendLine($"<div class='question-title'>{questionDetails?.Title ?? "Вопрос"}</div>");
                if (!string.IsNullOrEmpty(questionDetails?.Description))
                {
                    html.AppendLine($"<div style='font-size: 14px; color: #6c757d; margin-bottom: 8px;'>{questionDetails.Description}</div>");
                }
                html.AppendLine($"<div class='answer'>{displayAnswer}</div>");
                html.AppendLine("</div>");
            }

            html.AppendLine("<div class='footer'>");
            html.AppendLine("<p>Это автоматическое уведомление от системы FormFlow.</p>");
            html.AppendLine("</div>");
            html.AppendLine("</body></html>");

            return html.ToString();
        }

        private string FormatAnswerForEmail(object? answer, QuestionType questionType)
        {
            if (answer == null) return "<em>Не отвечено</em>";

            return questionType switch
            {
                QuestionType.ShortText or QuestionType.LongText => answer.ToString() ?? "",
                QuestionType.SingleChoice or QuestionType.Dropdown => answer.ToString() ?? "",
                QuestionType.MultipleChoice => answer.ToString() ?? "",
                QuestionType.Scale or QuestionType.Rating => $"{answer} из 5",
                QuestionType.Date => DateTime.TryParse(answer.ToString(), out var date) ? date.ToString("dd.MM.yyyy") : answer.ToString() ?? "",
                QuestionType.Time => answer.ToString() ?? "",
                _ => answer.ToString() ?? ""
            };
        }
    }
}

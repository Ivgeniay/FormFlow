using FormFlow.Domain.Models.General.QuestionDetailsModels;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Interfaces.Services;
using FormFlow.Domain.Models.General;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Text;
using System.Net;

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
                    Subject = $"Form answers: {template.Title}",
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
            var answers = JsonConvert.DeserializeObject<Dictionary<Guid, object>>(form.AnswersData) ?? new Dictionary<Guid, object>();
            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html><head><meta charset='utf-8'>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; color: #333; line-height: 1.6; }");
            html.AppendLine(".header { background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin-bottom: 30px; border: 1px solid #dee2e6; }");
            html.AppendLine(".header h2 { margin: 0 0 15px 0; color: #495057; }");
            html.AppendLine(".header p { margin: 5px 0; color: #6c757d; }");
            html.AppendLine(".question-block { margin-bottom: 25px; padding: 20px; border-left: 4px solid #007bff; background-color: #f8f9fa; border-radius: 0 8px 8px 0; }");
            html.AppendLine(".question-title { font-weight: bold; color: #495057; margin-bottom: 8px; font-size: 16px; }");
            html.AppendLine(".question-description { font-size: 14px; color: #6c757d; margin-bottom: 12px; font-style: italic; }");
            html.AppendLine(".answer { color: #212529; background-color: #ffffff; padding: 12px; border-radius: 4px; border: 1px solid #dee2e6; margin-top: 8px; }");
            html.AppendLine(".answer-label { font-weight: 600; color: #495057; margin-bottom: 4px; }");
            html.AppendLine(".no-answer { color: #6c757d; font-style: italic; }");
            html.AppendLine(".footer { margin-top: 40px; padding: 15px; background-color: #e9ecef; border-radius: 8px; font-size: 14px; color: #6c757d; text-align: center; }");
            html.AppendLine("</style></head><body>");

            html.AppendLine("<div class='header'>");
            html.AppendLine($"<h2>Form Responses: {template.Title}</h2>");
            html.AppendLine($"<p><strong>Description:</strong> {template.Description}</p>");
            html.AppendLine($"<p><strong>Author:</strong> {template.Author?.UserName}</p>");
            html.AppendLine($"<p><strong>Submitted:</strong> {form.SubmittedAt:dd/MM/yyyy HH:mm}</p>");
            html.AppendLine($"<p><strong>Respondent:</strong> {form.User?.UserName}</p>");
            html.AppendLine("</div>");

            foreach (var question in template.Questions.Where(q => !q.IsDeleted).OrderBy(q => q.Order))
            {
                var questionDetails = JsonConvert.DeserializeObject<QuestionDetails>(question.Data);
                var answerValue = answers.ContainsKey(question.Id) ? answers[question.Id] : null;
                var displayAnswer = FormatAnswerForEmail(answerValue, questionDetails);

                html.AppendLine("<div class='question-block'>");
                html.AppendLine($"<div class='question-title'>Question: {questionDetails?.Title ?? "Untitled Question"}</div>");

                if (!string.IsNullOrEmpty(questionDetails?.Description))
                {
                    html.AppendLine($"<div class='question-description'>{questionDetails.Description}</div>");
                }

                html.AppendLine("<div class='answer-label'>Answer:</div>");
                html.AppendLine($"<div class='answer'>{displayAnswer}</div>");
                html.AppendLine("</div>");
            }

            html.AppendLine("<div class='footer'>");
            html.AppendLine("<p>This is an automatic notification from the FormFlow system.</p>");
            html.AppendLine("<p>Please do not reply to this email.</p>");
            html.AppendLine("</div>");
            html.AppendLine("</body></html>");

            return html.ToString();
        }

        private string FormatAnswerForEmail(object? answer, QuestionDetails? questionDetails)
        {
            if (answer == null) return "<span class='no-answer'>No answer provided</span>";

            return questionDetails?.Type switch
            {
                QuestionType.ShortText => answer.ToString() ?? "",
                QuestionType.LongText => $"<div style='white-space: pre-wrap;'>{answer}</div>",
                QuestionType.SingleChoice => answer.ToString() ?? "",
                QuestionType.MultipleChoice => FormatMultipleChoiceAnswer(answer),
                QuestionType.Dropdown => answer.ToString() ?? "",
                QuestionType.Scale => FormatScaleAnswer(answer, questionDetails),
                QuestionType.Rating => FormatRatingAnswer(answer, questionDetails),
                QuestionType.Date => DateTime.TryParse(answer.ToString(), out var date) ? date.ToString("dd/MM/yyyy") : answer.ToString() ?? "",
                QuestionType.Time => answer.ToString() ?? "",
                _ => answer.ToString() ?? ""
            };
        }

        private string FormatScaleAnswer(object? answer, QuestionDetails? questionDetails)
        {
            if (answer == null) return "";

            var scaleData = questionDetails as dynamic;
            var maxValue = scaleData?.maxValue ?? 5;
            return $"{answer}/{maxValue}";
        }

        private string FormatRatingAnswer(object? answer, QuestionDetails? questionDetails)
        {
            if (answer == null) return "";

            var ratingData = questionDetails as dynamic;
            var maxRating = ratingData?.maxRating ?? 5;
            return $"{answer}/{maxRating} star{(answer.ToString() != "1" ? "s" : "")}";
        }

        private string FormatMultipleChoiceAnswer(object? answer)
        {
            if (answer == null) return "";

            try
            {
                var answerString = answer.ToString();
                if (answerString?.StartsWith("[") == true && answerString.EndsWith("]"))
                {
                    var options = JsonConvert.DeserializeObject<string[]>(answerString);
                    return string.Join(", ", options ?? Array.Empty<string>());
                }
                return answerString ?? "";
            }
            catch
            {
                return answer.ToString() ?? "";
            }
        }
    }
}

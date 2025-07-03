using FormFlow.Domain.Models.General.QuestionDetailsModels;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Application.DTOs.Templates;
using FormFlow.Application.Interfaces;
using FormFlow.Domain.Models.General;
using FormFlow.Domain.Exceptions;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;

namespace FormFlow.Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ITemplateRepository _templateRepository;
        private readonly IFormRepository _formRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ITopicRepository _topicRepository;

        public AnalyticsService(
            ITemplateRepository templateRepository,
            IFormRepository formRepository,
            IUserRepository userRepository,
            ITagRepository tagRepository,
            ILikeRepository likeRepository,
            ICommentRepository commentRepository,
            ITopicRepository topicRepository)
        {
            _templateRepository = templateRepository;
            _formRepository = formRepository;
            _userRepository = userRepository;
            _tagRepository = tagRepository;
            _likeRepository = likeRepository;
            _commentRepository = commentRepository;
            _topicRepository = topicRepository;
        }

        public async Task<BasicStatsDto> GetBasicStatsAsync(Guid templateId)
        {
            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            var forms = await _formRepository.GetFormsByTemplatePagedAsync(templateId, 1, int.MaxValue);
            var allForms = forms.Data;

            return new BasicStatsDto
            {
                TotalSubmissions = allForms.Count,
                FirstSubmission = allForms.Any() ? allForms.Min(f => f.SubmittedAt) : null,
                LastSubmission = allForms.Any() ? allForms.Max(f => f.SubmittedAt) : null
            };
        }

        public async Task<TimeAnalyticsDto> GetTimeAnalyticsAsync(Guid templateId, TimeAnalyticsQuery query)
        {
            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            var forms = await _formRepository.GetFormsByTemplatePagedAsync(templateId, 1, int.MaxValue);
            var allForms = forms.Data;

            var result = new TimeAnalyticsDto();

            if (query.Days != null)
            {
                result.SubmissionsByDay = GetSubmissionsByDay(allForms, query.Days);
            }

            if (query.Hours != null)
            {
                result.SubmissionsByHour = GetSubmissionsByHour(allForms, query.Hours);
            }

            if (query.Months != null)
            {
                result.SubmissionsByMonth = GetSubmissionsByMonth(allForms, query.Months);
            }

            return result;
        }

        private List<DaySubmissionDto> GetSubmissionsByDay(List<Form> forms, BetweenDays query)
        {
            var startDate = DateTime.Today.AddDays(-query.StartDaysAgo);
            var endDate = DateTime.Today.AddDays(-query.EndDaysAgo);

            var daysRange = Enumerable.Range(0, (endDate - startDate).Days + 1)
                .Select(i => startDate.AddDays(i))
                .ToList();

            var formsByDay = forms
                .Where(f => f.SubmittedAt.Date >= startDate && f.SubmittedAt.Date <= endDate)
                .GroupBy(f => f.SubmittedAt.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            return daysRange.Select(date => new DaySubmissionDto
            {
                Date = date,
                Count = formsByDay.ContainsKey(date) ? formsByDay[date] : 0
            }).ToList();
        }

        private List<HourSubmissionDto> GetSubmissionsByHour(List<Form> forms, BetweenHours query)
        {
            var hoursRange = Enumerable.Range(query.StartHour, query.EndHour - query.StartHour + 1).ToList();

            var formsByHour = forms
                .Where(f => f.SubmittedAt.Hour >= query.StartHour && f.SubmittedAt.Hour <= query.EndHour)
                .GroupBy(f => f.SubmittedAt.Hour)
                .ToDictionary(g => g.Key, g => g.Count());

            return hoursRange.Select(hour => new HourSubmissionDto
            {
                Hour = hour,
                Count = formsByHour.ContainsKey(hour) ? formsByHour[hour] : 0
            }).ToList();
        }

        private List<MonthSubmissionDto> GetSubmissionsByMonth(List<Form> forms, BetweenMonths query)
        {
            var startMonth = DateTime.Today.AddMonths(-query.StartMonthsAgo);
            var endMonth = DateTime.Today.AddMonths(-query.EndMonthsAgo);

            var totalMonths = ((endMonth.Year - startMonth.Year) * 12) + (endMonth.Month - startMonth.Month) + 1;
            var monthsRange = Enumerable.Range(0, totalMonths)
                .Select(i => new DateTime(startMonth.Year, startMonth.Month, 1).AddMonths(i))
                .ToList();

            var formsByMonth = forms
                .Where(f => new DateTime(f.SubmittedAt.Year, f.SubmittedAt.Month, 1) >= new DateTime(startMonth.Year, startMonth.Month, 1) &&
                           new DateTime(f.SubmittedAt.Year, f.SubmittedAt.Month, 1) <= new DateTime(endMonth.Year, endMonth.Month, 1))
                .GroupBy(f => new DateTime(f.SubmittedAt.Year, f.SubmittedAt.Month, 1))
                .ToDictionary(g => g.Key, g => g.Count());

            return monthsRange.Select(month => new MonthSubmissionDto
            {
                Month = month,
                Count = formsByMonth.ContainsKey(month) ? formsByMonth[month] : 0
            }).ToList();
        }
    }
}
 

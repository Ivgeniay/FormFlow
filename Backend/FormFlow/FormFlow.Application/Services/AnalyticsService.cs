using FormFlow.Domain.Models.General.QuestionDetailsModels;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Application.DTOs.Templates;
using FormFlow.Application.Interfaces;
using FormFlow.Domain.Models.General;
using FormFlow.Domain.Exceptions;
using System.Text.Json;
using System.Text;

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

        public async Task<TemplateAnalyticsDto> GetTemplateAnalyticsAsync(Guid templateId, Guid userId)
        {
            var template = await _templateRepository.GetWithAllDetailsAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            if (!await _templateRepository.HasUserAccessAsync(templateId, userId))
                throw new TemplateAccessDeniedException(templateId, userId);

            var forms = await _formRepository.GetFormsByTemplatePagedAsync(templateId, 1, int.MaxValue);
            var allForms = forms.Data;

            return new TemplateAnalyticsDto
            {
                TemplateId = templateId,
                TemplateName = template.Title,
                AuthorName = template.Author?.UserName ?? "Unknown",
                TotalSubmissions = allForms.Count,
                UniqueUsers = allForms.Select(f => f.UserId).Distinct().Count(),
                FirstSubmission = allForms.Any() ? allForms.Min(f => f.SubmittedAt) : null,
                LastSubmission = allForms.Any() ? allForms.Max(f => f.SubmittedAt) : null,
                AverageCompletionTime = CalculateAverageCompletionTime(allForms),
                SubmissionsByDay = GetSubmissionsByDay(allForms),
                SubmissionsByHour = GetSubmissionsByHour(allForms),
                TopUsers = GetTopUsers(allForms)
            };
        }

        public async Task<List<QuestionAnalyticsDto>> GetQuestionAnalyticsAsync(Guid templateId, Guid userId)
        {
            var template = await _templateRepository.GetWithQuestionsAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            if (!await _templateRepository.HasUserAccessAsync(templateId, userId))
                throw new TemplateAccessDeniedException(templateId, userId);

            var forms = await _formRepository.GetFormsByTemplatePagedAsync(templateId, 1, int.MaxValue);
            var allForms = forms.Data;

            var questionAnalytics = new List<QuestionAnalyticsDto>();

            foreach (var question in template.Questions.Where(q => !q.IsDeleted))
            {
                var analytics = AnalyzeQuestion(question, allForms);
                questionAnalytics.Add(analytics);
            }

            return questionAnalytics;
        }

        public async Task<QuestionAnalyticsDto> GetQuestionAnalyticsAsync(Guid templateId, Guid questionId, Guid userId)
        {
            var template = await _templateRepository.GetWithQuestionsAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            if (!await _templateRepository.HasUserAccessAsync(templateId, userId))
                throw new TemplateAccessDeniedException(templateId, userId);

            var question = template.Questions.FirstOrDefault(q => q.Id == questionId && !q.IsDeleted);
            if (question == null)
                throw new ArgumentException("Question not found");

            var forms = await _formRepository.GetFormsByTemplatePagedAsync(templateId, 1, int.MaxValue);

            return AnalyzeQuestion(question, forms.Data);
        }

        public async Task<AllVersionsAnalyticsDto> GetAllVersionsAnalyticsAsync(Guid baseTemplateId, Guid userId)
        {
            if (!await _templateRepository.IsAuthorOfBaseTemplateAsync(baseTemplateId, userId))
                throw new UnauthorizedAccessException("Only template author can view version analytics");

            var versions = await _templateRepository.GetAllVersionsAsync(baseTemplateId);
            if (!versions.Any())
                throw new TemplateNotFoundException(baseTemplateId);

            var templateIds = versions.Select(v => v.Id).ToList();
            var formsCountByTemplates = await _formRepository.GetFormsCountByTemplatesAsync(templateIds);

            var versionSummaries = new List<VersionAnalyticsSummaryDto>();
            var totalSubmissions = 0;
            var allUniqueUsers = new HashSet<Guid>();

            foreach (var version in versions)
            {
                var formsCount = formsCountByTemplates.ContainsKey(version.Id) ? formsCountByTemplates[version.Id] : 0;
                totalSubmissions += formsCount;

                if (formsCount > 0)
                {
                    var versionForms = await _formRepository.GetFormsByTemplatePagedAsync(version.Id, 1, int.MaxValue);
                    foreach (var form in versionForms.Data)
                    {
                        allUniqueUsers.Add(form.UserId);
                    }

                    versionSummaries.Add(new VersionAnalyticsSummaryDto
                    {
                        TemplateId = version.Id,
                        Version = version.Version,
                        Title = version.Title,
                        IsCurrentVersion = version.IsCurrentVersion,
                        FormsCount = formsCount,
                        UniqueUsers = versionForms.Data.Select(f => f.UserId).Distinct().Count(),
                        CreatedAt = version.CreatedAt,
                        FirstSubmission = versionForms.Data.Any() ? versionForms.Data.Min(f => f.SubmittedAt) : null,
                        LastSubmission = versionForms.Data.Any() ? versionForms.Data.Max(f => f.SubmittedAt) : null,
                        AverageCompletionTime = CalculateAverageCompletionTime(versionForms.Data)
                    });
                }
                else
                {
                    versionSummaries.Add(new VersionAnalyticsSummaryDto
                    {
                        TemplateId = version.Id,
                        Version = version.Version,
                        Title = version.Title,
                        IsCurrentVersion = version.IsCurrentVersion,
                        FormsCount = 0,
                        UniqueUsers = 0,
                        CreatedAt = version.CreatedAt,
                        FirstSubmission = null,
                        LastSubmission = null,
                        AverageCompletionTime = 0
                    });
                }
            }

            var firstVersion = versions.MinBy(v => v.CreatedAt);

            return new AllVersionsAnalyticsDto
            {
                BaseTemplateId = baseTemplateId,
                TemplateName = firstVersion?.Title ?? "Unknown Template",
                AuthorName = firstVersion?.Author?.UserName ?? "Unknown",
                TotalVersions = versions.Count,
                TotalSubmissionsAllVersions = totalSubmissions,
                TotalUniqueUsers = allUniqueUsers.Count,
                FirstVersionCreated = versions.Min(v => v.CreatedAt),
                LastVersionCreated = versions.Max(v => v.CreatedAt),
                VersionSummaries = versionSummaries.OrderBy(v => v.Version).ToList()
            };
        }

        public async Task<VersionComparisonDto> CompareVersionsAsync(Guid baseTemplateId, List<int> versions, Guid userId)
        {
            if (!await _templateRepository.IsAuthorOfBaseTemplateAsync(baseTemplateId, userId))
                throw new UnauthorizedAccessException("Only template author can compare versions");

            var allVersions = await _templateRepository.GetAllVersionsAsync(baseTemplateId);
            var targetVersions = allVersions.Where(v => versions.Contains(v.Version)).ToList();

            var templateIds = targetVersions.Select(v => v.Id).ToList();
            var formsCountByTemplates = await _formRepository.GetFormsCountByTemplatesAsync(templateIds);

            var versionSummaries = new List<VersionAnalyticsSummaryDto>();

            foreach (var version in targetVersions)
            {
                var formsCount = formsCountByTemplates.ContainsKey(version.Id) ? formsCountByTemplates[version.Id] : 0;

                versionSummaries.Add(new VersionAnalyticsSummaryDto
                {
                    TemplateId = version.Id,
                    Version = version.Version,
                    Title = version.Title,
                    IsCurrentVersion = version.IsCurrentVersion,
                    FormsCount = formsCount,
                    UniqueUsers = 0,
                    CreatedAt = version.CreatedAt,
                    FirstSubmission = null,
                    LastSubmission = null,
                    AverageCompletionTime = 0
                });
            }

            var insights = GenerateComparisonInsights(versionSummaries);

            return new VersionComparisonDto
            {
                BaseTemplateId = baseTemplateId,
                TemplateName = targetVersions.FirstOrDefault()?.Title ?? "Unknown",
                Versions = versionSummaries.OrderBy(v => v.Version).ToList(),
                Insights = insights,
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<List<TemplateListItemDto>> GetMostPopularTemplatesAsync(int count = 5)
        {
            var popularTemplates = await _templateRepository.GetPopularTemplatesAsync(1, count);
            return popularTemplates.Data.Select(s => MapToTemplateListItem(s)).ToList();
        }

        public async Task<List<TemplateListItemDto>> GetMostLikedTemplatesAsync(int count = 10)
        {
            var likedTemplates = await _likeRepository.GetMostLikedTemplatesAsync(count);
            return likedTemplates.Select(s => MapToTemplateListItem(s)).ToList();
        }

        public async Task<List<TemplateListItemDto>> GetLatestTemplatesAsync(int count = 10)
        {
            var latestTemplates = await _templateRepository.GetLatestTemplatesAsync(count);
            return latestTemplates.Select(s => MapToTemplateListItem(s)).ToList();
        }

        private TemplateListItemDto MapToTemplateListItem(Template s)
        {
            return new TemplateListItemDto
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description,
                TopicId = s.TopicId,
                ImageUrl = s.ImageUrl,
                AuthorName = s.Author?.UserName ?? string.Empty,
                CreatedAt = s.CreatedAt,
                Tags = s.Tags?.Select(tt => tt.Tag.Name).ToList() ?? new List<string>(),
                FormsCount = s.FormsCount,
                LikesCount = s.LikesCount
            };
        }

        public async Task<UserAnalyticsDto> GetUserAnalyticsAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            var userTemplates = await _templateRepository.GetTemplatesByAuthorPagedAsync(userId, 1, int.MaxValue);
            var userForms = await _formRepository.GetFormsByUserPagedAsync(userId, 1, int.MaxValue);

            var templateIds = userTemplates.Data.Select(t => t.Id).ToList();
            var likesCountByTemplates = await _likeRepository.GetLikesCountByTemplatesAsync(templateIds);
            var receivedLikes = likesCountByTemplates.Values.Sum();

            var givenLikes = await _likeRepository.GetCountByUserAsync(userId);
            var writtenComments = await _commentRepository.GetCountByUserAsync(userId);

            var mostUsedTags = await _tagRepository.GetMostUsedTagsByUserAsync(userId);
            var activityByMonth = GetUserActivityByMonth(userTemplates.Data, userForms.Data);

            return new UserAnalyticsDto
            {
                UserId = userId,
                UserName = user.UserName,
                RegistrationDate = user.CreatedAt,
                CreatedTemplates = userTemplates.Pagination.TotalCount,
                SubmittedForms = userForms.Pagination.TotalCount,
                ReceivedLikes = receivedLikes,
                GivenLikes = givenLikes,
                WrittenComments = writtenComments,
                MostUsedTags = mostUsedTags,
                ActivityByMonth = activityByMonth
            };
        }

        public async Task<SystemAnalyticsDto> GetSystemAnalyticsAsync()
        {
            var totalUsers = (await _userRepository.GetUsersPagedAsync(1, 1)).Pagination.TotalCount;
            var totalTemplates = (await _templateRepository.GetPublicTemplatesPagedAsync(1, 1)).Pagination.TotalCount;
            var totalForms = await _formRepository.GetTotalFormsCountAsync();
            var totalComments = await _commentRepository.GetTotalCommentsCountAsync();
            var totalLikes = await _likeRepository.GetTotalLikesCountAsync();

            var usersByMonth = await _userRepository.GetUsersCountByMonthAsync();
            var templatesCountByTopics = await _templateRepository.GetTemplatesCountByTopicsAsync();
            var allTopics = await _topicRepository.GetTopicsAsync(int.MaxValue, 1);

            var templatesByCategory = new Dictionary<string, int>();
            foreach (var topic in allTopics.Data)
            {
                var count = templatesCountByTopics.ContainsKey(topic.Id) ? templatesCountByTopics[topic.Id] : 0;
                templatesByCategory[topic.Name] = count;
            }

            var mostActiveUserIds = await _formRepository.GetMostActiveUsersAsync(5);
            var mostActiveUsersStats = await _userRepository.GetUserAnalyticsStatsAsync(mostActiveUserIds);
            var mostActiveUsers = mostActiveUsersStats
                .Select(u => $"{u.UserName} ({u.FormsCount} forms)")
                .ToList();

            return new SystemAnalyticsDto
            {
                TotalUsers = totalUsers,
                TotalTemplates = totalTemplates,
                TotalForms = totalForms,
                TotalComments = totalComments,
                TotalLikes = totalLikes,
                UsersByMonth = usersByMonth,
                TemplatesByCategory = templatesByCategory,
                MostActiveUsers = mostActiveUsers,
                SystemStartDate = DateTime.UtcNow.AddYears(-1)
            };
        }

        public async Task<TemplateComparisonDto> CompareTemplatesAsync(List<Guid> templateIds, Guid userId)
        {
            var templates = new List<TemplateComparisonItemDto>();

            var formsCountByTemplates = await _formRepository.GetFormsCountByTemplatesAsync(templateIds);
            var likesCountByTemplates = await _likeRepository.GetLikesCountByTemplatesAsync(templateIds);
            var commentsCountByTemplates = await _commentRepository.GetCommentsCountByTemplatesAsync(templateIds);

            foreach (var templateId in templateIds)
            {
                var template = await _templateRepository.GetByIdAsync(templateId);
                if (template != null && await _templateRepository.HasUserAccessAsync(templateId, userId))
                {
                    var formsCount = formsCountByTemplates.ContainsKey(templateId) ? formsCountByTemplates[templateId] : 0;
                    var likesCount = likesCountByTemplates.ContainsKey(templateId) ? likesCountByTemplates[templateId] : 0;
                    var commentsCount = commentsCountByTemplates.ContainsKey(templateId) ? commentsCountByTemplates[templateId] : 0;

                    templates.Add(new TemplateComparisonItemDto
                    {
                        TemplateId = templateId,
                        TemplateName = template.Title,
                        AuthorName = template.Author?.UserName ?? "Unknown",
                        FormsCount = formsCount,
                        LikesCount = likesCount,
                        CommentsCount = commentsCount,
                        AverageCompletionTime = 0,
                        CreatedAt = template.CreatedAt
                    });
                }
            }

            return new TemplateComparisonDto
            {
                Templates = templates,
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<byte[]> ExportCSVTemplateDataAsync(Guid templateId, Guid userId)
        {
            var template = await _templateRepository.GetWithQuestionsAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            if (!await _templateRepository.HasUserAccessAsync(templateId, userId))
                throw new TemplateAccessDeniedException(templateId, userId);

            var forms = await _formRepository.GetFormsByTemplatePagedAsync(templateId, 1, int.MaxValue);
            var csvContent = GenerateCSVContent(template, forms.Data);

            return Encoding.UTF8.GetBytes(csvContent);
        }

        public async Task<byte[]> ExportCSVAllVersionsDataAsync(Guid baseTemplateId, Guid userId)
        {
            if (!await _templateRepository.IsAuthorOfBaseTemplateAsync(baseTemplateId, userId))
                throw new UnauthorizedAccessException("Only template author can export version data");

            var versions = await _templateRepository.GetAllVersionsAsync(baseTemplateId);
            var allVersionData = new StringBuilder();

            allVersionData.AppendLine("Template Version,Template Title,User Name,Submitted At,Question Title,Answer");

            foreach (var version in versions)
            {
                var forms = await _formRepository.GetFormsByTemplatePagedAsync(version.Id, 1, int.MaxValue);

                foreach (var form in forms.Data)
                {
                    var answers = JsonSerializer.Deserialize<Dictionary<Guid, object>>(form.AnswersData) ?? new Dictionary<Guid, object>();

                    foreach (var question in version.Questions.Where(q => !q.IsDeleted))
                    {
                        var questionDetails = JsonSerializer.Deserialize<QuestionDetails>(question.Data);
                        var answer = answers.ContainsKey(question.Id) ? answers[question.Id]?.ToString() ?? "" : "";

                        allVersionData.AppendLine($"{version.Version},{EscapeCsv(version.Title)},{EscapeCsv(form.User?.UserName ?? "Unknown")},{form.SubmittedAt:yyyy-MM-dd HH:mm},{EscapeCsv(questionDetails?.Title ?? "Unknown")},{EscapeCsv(answer)}");
                    }
                }
            }

            return Encoding.UTF8.GetBytes(allVersionData.ToString());
        }

        private QuestionAnalyticsDto AnalyzeQuestion(Question question, List<Form> forms)
        {
            var questionDetails = JsonSerializer.Deserialize<QuestionDetails>(question.Data);
            var questionType = questionDetails?.Type ?? QuestionType.ShortText;

            var answers = new List<object>();
            var totalAnswers = 0;
            var skippedAnswers = 0;

            foreach (var form in forms)
            {
                var formAnswers = JsonSerializer.Deserialize<Dictionary<Guid, object>>(form.AnswersData) ?? new Dictionary<Guid, object>();

                if (formAnswers.ContainsKey(question.Id) && formAnswers[question.Id] != null)
                {
                    answers.Add(formAnswers[question.Id]);
                    totalAnswers++;
                }
                else
                {
                    skippedAnswers++;
                }
            }

            var responseRate = forms.Count > 0 ? (double)totalAnswers / forms.Count * 100 : 0;

            var analytics = new QuestionAnalyticsDto
            {
                QuestionId = question.Id,
                QuestionTitle = questionDetails?.Title ?? "Unknown Question",
                QuestionType = questionType,
                TotalAnswers = totalAnswers,
                SkippedAnswers = skippedAnswers,
                ResponseRate = responseRate
            };

            switch (questionType)
            {
                case QuestionType.ShortText:
                case QuestionType.LongText:
                    analytics.TextAnalytics = AnalyzeTextAnswers(answers);
                    break;
                case QuestionType.Scale:
                case QuestionType.Rating:
                    analytics.NumericAnalytics = AnalyzeNumericAnswers(answers);
                    analytics.ScaleAnalytics = AnalyzeScaleAnswers(answers);
                    break;
                case QuestionType.SingleChoice:
                case QuestionType.MultipleChoice:
                case QuestionType.Dropdown:
                    analytics.ChoiceAnalytics = AnalyzeChoiceAnswers(answers);
                    break;
            }

            return analytics;
        }

        private TextAnalyticsDto AnalyzeTextAnswers(List<object> answers)
        {
            var textAnswers = answers
                .Select(a => a?
                .ToString() ?? "")
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();

            if (!textAnswers.Any())
                return new TextAnalyticsDto();

            var lengths = textAnswers.Select(t => t.Length).ToList();
            var allWords = textAnswers
                .SelectMany(t => t.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .Where(w => w.Length > 2)
                .Select(w => w.ToLower())
                .ToList();

            var wordFrequency = allWords
                .GroupBy(w => w)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => new WordFrequencyDto
                {
                    Word = g.Key,
                    Frequency = g.Count(),
                    Percentage = (double)g.Count() / allWords.Count * 100
                })
                .ToList();

            return new TextAnalyticsDto
            {
                AverageLength = lengths.Average(),
                MaxLength = lengths.Max(),
                MinLength = lengths.Min(),
                MostCommonWords = wordFrequency,
                LongestAnswers = textAnswers.OrderByDescending(t => t.Length).Take(3).ToList(),
                ShortestAnswers = textAnswers.OrderBy(t => t.Length).Take(3).ToList()
            };
        }

        private NumericAnalyticsDto AnalyzeNumericAnswers(List<object> answers)
        {
            var numericAnswers = answers
                .Select(a => {
                    if (int.TryParse(a?.ToString(), out int result))
                        return (int?)result;
                    return null;
                })
                .Where(n => n.HasValue)
                .Select(n => n.Value)
                .ToList();

            if (!numericAnswers.Any())
                return new NumericAnalyticsDto();

            var sorted = numericAnswers.OrderBy(n => n).ToList();
            var median = sorted.Count % 2 == 0
                ? (sorted[sorted.Count / 2 - 1] + sorted[sorted.Count / 2]) / 2.0
                : sorted[sorted.Count / 2];

            var average = numericAnswers.Average();
            var variance = numericAnswers.Select(n => Math.Pow(n - average, 2)).Average();
            var standardDeviation = Math.Sqrt(variance);

            var distribution = numericAnswers
                .GroupBy(n => n)
                .ToDictionary(g => g.Key, g => g.Count());

            return new NumericAnalyticsDto
            {
                Average = average,
                Median = median,
                Minimum = numericAnswers.Min(),
                Maximum = numericAnswers.Max(),
                StandardDeviation = standardDeviation,
                Distribution = distribution
            };
        }

        private ScaleAnalyticsDto AnalyzeScaleAnswers(List<object> answers)
        {
            var scaleAnswers = answers
                .Select(a => {
                    if (int.TryParse(a?.ToString(), out int result))
                        return (int?)result;
                    return null;
                })
                .Where(n => n.HasValue)
                .Select(n => n.Value)
                .ToList();

            if (!scaleAnswers.Any())
                return new ScaleAnalyticsDto();

            var sorted = scaleAnswers.OrderBy(n => n).ToList();
            var median = sorted.Count % 2 == 0
                ? (sorted[sorted.Count / 2 - 1] + sorted[sorted.Count / 2]) / 2.0
                : sorted[sorted.Count / 2];

            var distribution = scaleAnswers
                .GroupBy(n => n)
                .ToDictionary(g => g.Key, g => g.Count());

            var percentages = distribution
                .ToDictionary(kvp => kvp.Key, kvp => (double)kvp.Value / scaleAnswers.Count * 100);

            var mostCommon = distribution.Any() ? distribution.OrderByDescending(kvp => kvp.Value).First().Key : 0;

            return new ScaleAnalyticsDto
            {
                Average = scaleAnswers.Average(),
                Median = median,
                RatingDistribution = distribution,
                RatingPercentages = percentages,
                MostCommonRating = mostCommon
            };
        }

        private ChoiceAnalyticsDto AnalyzeChoiceAnswers(List<object> answers)
        {
            var choiceAnswers = answers.Select(a => a?.ToString() ?? "").Where(s => !string.IsNullOrEmpty(s)).ToList();

            if (!choiceAnswers.Any())
                return new ChoiceAnalyticsDto();

            var optionCounts = choiceAnswers
                .GroupBy(a => a)
                .ToDictionary(g => g.Key, g => g.Count());

            var optionPercentages = optionCounts
                .ToDictionary(kvp => kvp.Key, kvp => (double)kvp.Value / choiceAnswers.Count * 100);

            var mostPopular = optionCounts.Any() ? optionCounts.OrderByDescending(kvp => kvp.Value).First().Key : "";
            var leastPopular = optionCounts.Any() ? optionCounts.OrderBy(kvp => kvp.Value).First().Key : "";

            return new ChoiceAnalyticsDto
            {
                OptionCounts = optionCounts,
                OptionPercentages = optionPercentages,
                MostPopularOption = mostPopular,
                LeastPopularOption = leastPopular
            };
        }

        private double CalculateAverageCompletionTime(List<Form> forms)
        {
            if (!forms.Any())
                return 0;

            var completionTimes = forms
                .Where(f => f.UpdatedAt != f.SubmittedAt)
                .Select(f => (f.UpdatedAt - f.SubmittedAt).TotalMinutes)
                .Where(t => t > 0 && t < 60)
                .ToList();

            return completionTimes.Any() ? completionTimes.Average() : 5.0;
        }

        private Dictionary<string, int> GetSubmissionsByDay(List<Form> forms)
        {
            return forms
                .GroupBy(f => f.SubmittedAt.Date.ToString("yyyy-MM-dd"))
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private Dictionary<string, int> GetSubmissionsByHour(List<Form> forms)
        {
            return forms
                .GroupBy(f => f.SubmittedAt.Hour.ToString("00") + ":00")
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private List<string> GetTopUsers(List<Form> forms)
        {
            return forms
                .GroupBy(f => f.User?.UserName ?? "Unknown")
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => $"{g.Key} ({g.Count()} forms)")
                .ToList();
        }

        private VersionComparisonInsightsDto GenerateComparisonInsights(List<VersionAnalyticsSummaryDto> versions)
        {
            if (!versions.Any())
                return new VersionComparisonInsightsDto();

            var mostSuccessful = versions.OrderByDescending(v => v.FormsCount).First();
            var leastSuccessful = versions.OrderBy(v => v.FormsCount).First();

            var recommendations = new List<string>
            {
                "Analyze user feedback from the most successful version",
                "Consider rolling back problematic changes from low-performing versions",
                "Focus on features that increased completion rates"
            };

            return new VersionComparisonInsightsDto
            {
                MostSuccessfulVersion = mostSuccessful.Version,
                MostSuccessfulReason = $"Highest number of submissions ({mostSuccessful.FormsCount})",
                LeastSuccessfulVersion = leastSuccessful.Version,
                LeastSuccessfulReason = $"Lowest number of submissions ({leastSuccessful.FormsCount})",
                AverageImprovementPerVersion = versions.Count > 1 ?
                    (versions.Last().FormsCount - versions.First().FormsCount) / (double)(versions.Count - 1) : 0,
                Recommendations = recommendations
            };
        }

        private Dictionary<string, int> GetUserActivityByMonth(List<Template> templates, List<Form> forms)
        {
            var templateActivity = templates
                .GroupBy(t => t.CreatedAt.ToString("yyyy-MM"))
                .ToDictionary(g => g.Key, g => g.Count());

            var formActivity = forms
                .GroupBy(f => f.SubmittedAt.ToString("yyyy-MM"))
                .ToDictionary(g => g.Key, g => g.Count());

            var allMonths = templateActivity.Keys.Union(formActivity.Keys);
            var combinedActivity = new Dictionary<string, int>();

            foreach (var month in allMonths)
            {
                var templateCount = templateActivity.ContainsKey(month) ? templateActivity[month] : 0;
                var formCount = formActivity.ContainsKey(month) ? formActivity[month] : 0;
                combinedActivity[month] = templateCount + formCount;
            }

            return combinedActivity;
        }

        private string GenerateCSVContent(Template template, List<Form> forms)
        {
            var csv = new StringBuilder();

            var headers = new List<string> { "User", "Submitted At" };
            var questions = template.Questions.Where(q => !q.IsDeleted).OrderBy(q => q.Order).ToList();

            foreach (var question in questions)
            {
                var questionDetails = JsonSerializer.Deserialize<QuestionDetails>(question.Data);
                headers.Add(questionDetails?.Title ?? "Unknown Question");
            }

            csv.AppendLine(string.Join(",", headers.Select(EscapeCsv)));

            foreach (var form in forms)
            {
                var row = new List<string>
                {
                    form.User?.UserName ?? "Unknown",
                    form.SubmittedAt.ToString("yyyy-MM-dd HH:mm")
                };

                var answers = JsonSerializer.Deserialize<Dictionary<Guid, object>>(form.AnswersData) ?? new Dictionary<Guid, object>();

                foreach (var question in questions)
                {
                    var answer = answers.ContainsKey(question.Id) ?
                        answers[question.Id]?.ToString() ?? "" : "";
                    row.Add(answer);
                }

                csv.AppendLine(string.Join(",", row.Select(EscapeCsv)));
            }

            return csv.ToString();
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "\"\"";

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return "\"" + value + "\"";
        }
    }
}
 

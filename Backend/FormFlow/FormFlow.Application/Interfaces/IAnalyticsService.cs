using FormFlow.Application.DTOs.Templates;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Interfaces
{
    public interface IAnalyticsService
    {
        Task<TemplateAnalyticsDto> GetTemplateAnalyticsAsync(Guid templateId, Guid userId);
        Task<List<QuestionAnalyticsDto>> GetQuestionAnalyticsAsync(Guid templateId, Guid userId);
        Task<QuestionAnalyticsDto> GetQuestionAnalyticsAsync(Guid templateId, Guid questionId, Guid userId);

        Task<AllVersionsAnalyticsDto> GetAllVersionsAnalyticsAsync(Guid baseTemplateId, Guid userId);
        Task<VersionComparisonDto> CompareVersionsAsync(Guid baseTemplateId, List<int> versions, Guid userId);

        Task<List<TemplateListItemDto>> GetMostPopularTemplatesAsync(int count = 5);
        Task<List<TemplateListItemDto>> GetMostLikedTemplatesAsync(int count = 10);
        Task<List<TemplateListItemDto>> GetLatestTemplatesAsync(int count = 10);

        Task<UserAnalyticsDto> GetUserAnalyticsAsync(Guid userId);
        Task<SystemAnalyticsDto> GetSystemAnalyticsAsync();

        Task<TemplateComparisonDto> CompareTemplatesAsync(List<Guid> templateIds, Guid userId);

        Task<byte[]> ExportCSVTemplateDataAsync(Guid templateId, Guid userId);
        Task<byte[]> ExportCSVAllVersionsDataAsync(Guid baseTemplateId, Guid userId);
    }

    public class UserAnalyticsDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }

        public int CreatedTemplates { get; set; }
        public int SubmittedForms { get; set; }
        public int ReceivedLikes { get; set; }
        public int GivenLikes { get; set; }
        public int WrittenComments { get; set; }

        public List<string> MostUsedTags { get; set; } = new();
        public Dictionary<string, int> ActivityByMonth { get; set; } = new();
    }

    public class TemplateAnalyticsDto
    {
        public Guid TemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;

        public int TotalSubmissions { get; set; }
        public int UniqueUsers { get; set; }
        public DateTime? FirstSubmission { get; set; }
        public DateTime? LastSubmission { get; set; }
        public double AverageCompletionTime { get; set; }

        public Dictionary<string, int> SubmissionsByDay { get; set; } = new();
        public Dictionary<string, int> SubmissionsByHour { get; set; } = new();
        public List<string> TopUsers { get; set; } = new(); 
    }

    public class QuestionAnalyticsDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionTitle { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public int TotalAnswers { get; set; }
        public int SkippedAnswers { get; set; }
        public double ResponseRate { get; set; }

        public TextAnalyticsDto? TextAnalytics { get; set; }
        public NumericAnalyticsDto? NumericAnalytics { get; set; }
        public ChoiceAnalyticsDto? ChoiceAnalytics { get; set; }
        public ScaleAnalyticsDto? ScaleAnalytics { get; set; }
    }

    public class TextAnalyticsDto
    {
        public double AverageLength { get; set; }
        public int MaxLength { get; set; }
        public int MinLength { get; set; }
        public List<WordFrequencyDto> MostCommonWords { get; set; } = new();
        public List<string> LongestAnswers { get; set; } = new();
        public List<string> ShortestAnswers { get; set; } = new();
    }

    public class ChoiceAnalyticsDto
    {
        public Dictionary<string, int> OptionCounts { get; set; } = new();
        public Dictionary<string, double> OptionPercentages { get; set; } = new();
        public string MostPopularOption { get; set; } = string.Empty;
        public string LeastPopularOption { get; set; } = string.Empty;
    }

    public class ScaleAnalyticsDto
    {
        public double Average { get; set; }
        public double Median { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
        public Dictionary<int, double> RatingPercentages { get; set; } = new();
        public int MostCommonRating { get; set; }
    }

    public class WordFrequencyDto
    {
        public string Word { get; set; } = string.Empty;
        public int Frequency { get; set; }
        public double Percentage { get; set; }
    }

    public class NumericAnalyticsDto
    {
        public double Average { get; set; }
        public double Median { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public double StandardDeviation { get; set; }
        public Dictionary<int, int> Distribution { get; set; } = new();
    }

    public class SystemAnalyticsDto
    {
        public int TotalUsers { get; set; }
        public int TotalTemplates { get; set; }
        public int TotalForms { get; set; }
        public int TotalComments { get; set; }
        public int TotalLikes { get; set; }

        public Dictionary<string, int> UsersByMonth { get; set; } = new();
        public Dictionary<string, int> TemplatesByCategory { get; set; } = new();
        public List<string> MostActiveUsers { get; set; } = new();
        public DateTime SystemStartDate { get; set; }
    }

    public class TemplateComparisonItemDto
    {
        public Guid TemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public int FormsCount { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public double AverageCompletionTime { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TemplateComparisonDto
    {
        public List<TemplateComparisonItemDto> Templates { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
    }

    public class AllVersionsAnalyticsDto
    {
        public Guid BaseTemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public int TotalVersions { get; set; }
        public int TotalSubmissionsAllVersions { get; set; }
        public int TotalUniqueUsers { get; set; }
        public DateTime FirstVersionCreated { get; set; }
        public DateTime LastVersionCreated { get; set; }

        public List<VersionAnalyticsSummaryDto> VersionSummaries { get; set; } = new List<VersionAnalyticsSummaryDto>();
        public Dictionary<string, int> SubmissionsByVersionAndDay { get; set; } = new();
    }

    public class VersionAnalyticsSummaryDto
    {
        public Guid TemplateId { get; set; }
        public int Version { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCurrentVersion { get; set; }
        public int FormsCount { get; set; }
        public int UniqueUsers { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FirstSubmission { get; set; }
        public DateTime? LastSubmission { get; set; }
        public double AverageCompletionTime { get; set; }
    }

    public class VersionComparisonDto
    {
        public Guid BaseTemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public List<VersionAnalyticsSummaryDto> Versions { get; set; } = new List<VersionAnalyticsSummaryDto>();
        public VersionComparisonInsightsDto Insights { get; set; } = new VersionComparisonInsightsDto();
        public DateTime GeneratedAt { get; set; }
    }

    public class VersionComparisonInsightsDto
    {
        public int MostSuccessfulVersion { get; set; }
        public string MostSuccessfulReason { get; set; } = string.Empty;
        public int LeastSuccessfulVersion { get; set; }
        public string LeastSuccessfulReason { get; set; } = string.Empty;
        public double AverageImprovementPerVersion { get; set; }
        public List<string> Recommendations { get; set; } = new List<string>();
    }
}

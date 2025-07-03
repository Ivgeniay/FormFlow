namespace FormFlow.Application.Interfaces
{
    public interface IAnalyticsService
    {
        Task<BasicStatsDto> GetBasicStatsAsync(Guid templateId);
        Task<TimeAnalyticsDto> GetTimeAnalyticsAsync(Guid templateId, TimeAnalyticsQuery query);
    }

    public class BasicStatsDto
    {
        public int TotalSubmissions { get; set; }
        public DateTime? FirstSubmission { get; set; }
        public DateTime? LastSubmission { get; set; }
    }

    public class TimeAnalyticsQuery
    {
        public BetweenDays? Days { get; set; }
        public BetweenHours? Hours { get; set; }
        public BetweenMonths? Months { get; set; }
    }

    public class BetweenDays 
    {
        public int StartDaysAgo { get; set; }
        public int EndDaysAgo { get; set; }
    }

    public class BetweenHours
    {
        public int StartHour { get; set; }
        public int EndHour { get; set; }
    }

    public class BetweenMonths
    {
        public int StartMonthsAgo { get; set; }
        public int EndMonthsAgo { get; set; }
    }

    public class TimeAnalyticsDto
    {
        public List<DaySubmissionDto> SubmissionsByDay { get; set; } = new();
        public List<HourSubmissionDto> SubmissionsByHour { get; set; } = new();
        public List<MonthSubmissionDto> SubmissionsByMonth { get; set; } = new();
    }

    public class DaySubmissionDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class HourSubmissionDto
    {
        public int Hour { get; set; }
        public int Count { get; set; }
    }

    public class MonthSubmissionDto
    {
        public DateTime Month { get; set; }
        public int Count { get; set; }
    }
}

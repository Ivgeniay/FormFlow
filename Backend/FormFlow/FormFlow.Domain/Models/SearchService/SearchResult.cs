namespace FormFlow.Domain.Models.SearchService
{
    public class SearchResult<T>
    {
        public List<T> Results { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Query { get; set; } = string.Empty;
        public TimeSpan SearchTime { get; set; }
    }
}

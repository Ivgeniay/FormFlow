namespace FormFlow.Domain.Models.SearchService
{
    public class SearchQuery
    {
        public string Query { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public List<string>? Tags { get; set; }
        public string? AuthorName { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public SearchSortBy SortBy { get; set; } = SearchSortBy.Relevance;
    }
}

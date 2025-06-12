using FormFlow.Domain.Models.SearchService;

namespace FormFlow.Domain.Interfaces.Services
{
    public interface ISearchService
    {
        Task<SearchResult<TemplateSearchDocument>> SearchTemplatesAsync(SearchQuery query);
        Task<List<string>> GetSearchSuggestionsAsync(string query, int limit = 10);

        Task IndexTemplateAsync(TemplateSearchDocument document);
        Task UpdateTemplateIndexAsync(TemplateSearchDocument document);
        Task RemoveTemplateFromIndexAsync(Guid templateId);
        Task ReindexAllTemplatesAsync();
    }
}

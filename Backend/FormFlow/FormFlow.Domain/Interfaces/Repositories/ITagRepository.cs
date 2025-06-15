using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Interfaces.Repositories
{
    public interface ITagRepository
    {
        Task<Tag?> GetByIdAsync(Guid id);
        Task<Tag?> GetByNameAsync(string name);
        Task<Dictionary<string, Guid>> GetTagIdsByNamesAsync(List<string> names);
        Task<Tag> CreateAsync(Tag tag);
        Task<Tag> UpdateAsync(Tag tag);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> NameExistsAsync(string name);
        Task DeleteAsync(Guid id);

        Task<PagedResult<Tag>> GetTagsPagedAsync(int page, int pageSize);
        Task<List<Tag>> GetMostPopularAsync(int count = 50);
        Task<List<Tag>> SearchByNameAsync(string query, int limit = 10);

        Task<Tag> GetOrCreateByNameAsync(string name);
        Task<List<Tag>> GetOrCreateByNamesAsync(List<string> names);

        Task IncrementUsageCountAsync(Guid tagId);
        Task DecrementUsageCountAsync(Guid tagId);
        Task RecalculateUsageCountAsync(Guid tagId);
        Task CleanupUnusedTagsAsync();

        Task<Dictionary<Guid, List<string>>> GetTagsByTemplatesAsync(List<Guid> templateIds);
        Task<List<string>> GetMostUsedTagsByUserAsync(Guid userId);
    }
}

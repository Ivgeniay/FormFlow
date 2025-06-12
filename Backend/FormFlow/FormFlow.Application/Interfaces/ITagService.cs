using FormFlow.Application.DTOs.Tags;
using FormFlow.Application.DTOs.Templates;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Interfaces
{
    public interface ITagService
    {
        Task<TagDto> CreateTagAsync(string name);
        Task<TagDto> GetTagByIdAsync(Guid id);
        Task<TagDto> GetTagByNameAsync(string name);
        Task<bool> TagExistsAsync(string name);

        Task<PagedResult<TagDto>> GetTagsPagedAsync(int page, int pageSize);
        Task<List<TagDto>> GetMostPopularTagsAsync(int count = 50);
        Task<List<TagDto>> SearchTagsAsync(string query, int limit = 10);

        Task<List<TagDto>> GetOrCreateTagsAsync(List<string> tagNames);
        Task<TagDto> GetOrCreateTagAsync(string name);

        Task UpdateTagUsageCountAsync(Guid tagId);
        Task IncrementTagUsageAsync(Guid tagId);
        Task DecrementTagUsageAsync(Guid tagId);
        Task RecalculateTagUsageAsync(Guid tagId);

        Task<List<TemplateListItemDto>> GetTemplatesByTagAsync(Guid tagId, int page, int pageSize);
        Task<List<TemplateListItemDto>> GetTemplatesByTagNameAsync(string tagName, int page, int pageSize);

        Task CleanupUnusedTagsAsync();
        Task<CloudTagDto> GetTagCloudAsync(int maxTags = 50);
    }
}

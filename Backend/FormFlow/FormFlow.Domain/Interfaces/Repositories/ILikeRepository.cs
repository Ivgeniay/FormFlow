using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Interfaces.Repositories
{
    public interface ILikeRepository
    {
        Task<Like?> GetByIdAsync(Guid id);
        Task<Like> CreateAsync(Like like);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);

        Task<Like?> GetByTemplateAndUserAsync(Guid templateId, Guid userId);
        Task<PagedResult<Like>> GetLikesByTemplatePagedAsync(Guid templateId, int page, int pageSize);
        Task<PagedResult<Like>> GetLikesByUserPagedAsync(Guid userId, int page, int pageSize);

        Task<bool> HasUserLikedAsync(Guid templateId, Guid userId);
        Task<int> GetCountByTemplateAsync(Guid templateId);
        Task<int> GetCountByUserAsync(Guid userId);

        Task<Like> ToggleLikeAsync(Guid templateId, Guid userId);
        Task<bool> AddLikeAsync(Guid templateId, Guid userId);
        Task<bool> RemoveLikeAsync(Guid templateId, Guid userId);

        Task<List<Template>> GetMostLikedTemplatesAsync(int count = 10);
    }
}

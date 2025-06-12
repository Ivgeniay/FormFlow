using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Interfaces.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(Guid id);
        Task<Comment> CreateAsync(Comment comment);
        Task<Comment> UpdateAsync(Comment comment);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);

        Task<PagedResult<Comment>> GetCommentsByTemplatePagedAsync(Guid templateId, int page, int pageSize);
        Task<PagedResult<Comment>> GetCommentsByUserPagedAsync(Guid userId, int page, int pageSize);
        Task<List<Comment>> GetRecentByTemplateAsync(Guid templateId, int count = 10);

        Task<Comment?> GetWithUserAsync(Guid id);

        Task<int> GetCountByTemplateAsync(Guid templateId);
        Task<int> GetCountByUserAsync(Guid userId);

        Task<bool> CanUserDeleteAsync(Guid commentId, Guid userId);
        Task<DateTime?> GetLastCommentTimeAsync(Guid templateId);
    }
}

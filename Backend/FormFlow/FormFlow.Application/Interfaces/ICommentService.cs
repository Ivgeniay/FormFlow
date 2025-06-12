using FormFlow.Application.DTOs.Comments;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Interfaces
{
    public interface ICommentService
    {
        Task<CommentDto> AddCommentAsync(Guid userId, AddCommentRequest request);
        Task DeleteCommentAsync(Guid commentId, Guid userId);
        Task<CommentDto> GetCommentByIdAsync(Guid commentId);
        Task<bool> CommentExistsAsync(Guid commentId);

        Task<PagedResult<CommentDto>> GetCommentsPagedAsync(Guid templateId, int page, int pageSize);
        Task<List<CommentDto>> GetRecentCommentsAsync(Guid templateId, int count = 20);
        Task<int> GetCommentsCountAsync(Guid templateId);

        Task<bool> CanDeleteCommentAsync(Guid commentId, Guid userId);
        Task<bool> CanUserCommentAsync(Guid templateId, Guid userId);
    }
}

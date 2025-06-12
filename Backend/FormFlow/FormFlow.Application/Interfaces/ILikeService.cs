using FormFlow.Application.DTOs.Likes;
using FormFlow.Application.DTOs.Templates;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Interfaces
{
    public interface ILikeService
    {
        Task<LikeResultDto> ToggleLikeAsync(Guid userId, Guid templateId);
        Task<bool> AddLikeAsync(Guid userId, Guid templateId);
        Task<bool> RemoveLikeAsync(Guid userId, Guid templateId);

        Task<bool> HasUserLikedAsync(Guid userId, Guid templateId);
        Task<int> GetLikesCountAsync(Guid templateId);
        Task<PagedResult<LikeDto>> GetLikesPagedAsync(Guid templateId, int page, int pageSize);

        Task<PagedResult<TemplateListItemDto>> GetUserLikedTemplatesPagedAsync(Guid userId, int page, int pageSize);
    }
}

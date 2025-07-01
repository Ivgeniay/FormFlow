using FormFlow.Application.DTOs.Templates;
using FormFlow.Application.DTOs.Users;
using FormFlow.Domain.Models.General;
using System.Numerics;

namespace FormFlow.Application.Interfaces
{
    public interface ITemplateService
    {
        Task<TemplateDto> CreateTemplateAsync(CreateTemplateRequest request, Guid authorId);
        Task<TemplateDto> CreateNewVersionAsync(CreateNewVersionRequest request, Guid userId);
        Task<TemplateDto> UpdateTemplateAsync(UpdateTemplateRequest request, Guid userId);
        
        Task<TemplateDto> PublishTemplateAsync(Guid templateId, Guid userId);
        Task<TemplateDto> ArchiveTemplateAsync(Guid templateId, Guid userId);
        Task<bool> ArchiveTemplatesAsync(Guid[] templateIds, Guid userId);
        Task<bool> UnarchiveTemplatesAsync(Guid[] templateIds, Guid userId);
        Task DeleteTemplateAsync(Guid templateId, Guid userId);
        Task UnDeleteTemplateAsync(Guid templateId, Guid userId);
        Task<bool> DeleteTemplatesAsync(Guid[] templateIds, Guid userId);
        Task DeleteAllVersionsAsync(Guid baseTemplateId, Guid userId);

        Task<TemplateDto> GetTemplateByIdAsync(Guid id, Guid? userId = null);
        Task<List<TemplateDto>> GetTemplateByIdsAsync(IEnumerable<Guid> ids, Guid? userId = null);
        Task<TemplateDto> GetCurrentVersionAsync(Guid baseTemplateId, Guid? userId = null);
        Task<TemplateDto> GetSpecificVersionAsync(Guid baseTemplateId, int version, Guid? userId = null);
        Task<List<TemplateDto>> GetAllVersionsForUserAsync(Guid templateId, Guid askingUserId, Guid forUserId);
        Task<List<TemplateDto>> GetAllVersionsAsync(Guid templateId, Guid askingUserId);
        Task<bool> TemplateExistsAsync(Guid id);
        Task<bool> BaseTemplateExistsAsync(Guid baseTemplateId);

        Task<PagedResult<TemplateDto>> GetTemplatesPagedForAdminAsync(int page, int pageSize);
        Task<PagedResult<TemplateDto>> GetPublicTemplatesPagedAsync(int page, int pageSize);
        Task<PagedResult<TemplateDto>> GetTemplatesByTagNameAsync(string tagName, int page, int pageSize);
        Task<PagedResult<TemplateDto>> GetPopularTemplatesAsync(int page, int pageSize);
        Task<PagedResult<TemplateDto>> GetTemplatesByAuthorPagedAsync(Guid authorId, int page, int pageSize);
        Task<PagedResult<TemplateDto>> GetUserAccessibleTemplatesPagedAsync(Guid userId, int page, int pageSize);
        Task<List<TemplateDto>> GetLatestTemplatesAsync(int count = 10);

        Task AddTagToTemplateAsync(Guid templateId, string tagName, Guid userId);
        Task RemoveTagFromTemplateAsync(Guid templateId, Guid tagId, Guid userId);

        Task<TemplateDto> UpdateTemplateTagsAsync(Guid templateId, UpdateTemplateTagsRequest request, Guid userId);
        Task<TemplateDto> UpdateTemplateAllowedUsersAsync(Guid templateId, UpdateTemplateAllowedUsersRequest request, Guid userId);

        Task<bool> UpdateTemplateImageAsync(Guid templateId, string imageUrl, Guid userId);

        Task AddAllowedUserToTemplateAsync(Guid templateId, Guid allowedUserId, Guid ownerId);
        Task RemoveAllowedUserFromTemplateAsync(Guid templateId, Guid allowedUserId, Guid ownerId);
        Task<List<UserSearchDto>> GetTemplateAllowedUsersAsync(Guid templateId, Guid userId);

        Task<bool> HasUserAccessToTemplateAsync(Guid templateId, Guid userId);
        Task<bool> IsUserTemplateAuthorAsync(Guid templateId, Guid userId);
        Task<bool> IsUserBaseTemplateAuthorAsync(Guid baseTemplateId, Guid userId);
        Task<bool> CanUserEditTemplateAsync(Guid templateId, Guid userId);
        Task<bool> CanUserCreateNewVersionAsync(Guid baseTemplateId, Guid userId);

        Task RemoveTemplateImageAsync(Guid templateId, Guid userId);

        Task<TemplateVersionInfoDto> GetVersionInfoAsync(Guid baseTemplateId, Guid userId);
    }
}

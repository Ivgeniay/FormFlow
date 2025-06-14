﻿using FormFlow.Application.DTOs.Templates;
using FormFlow.Application.DTOs.Users;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Interfaces
{
    public interface ITemplateService
    {
        Task<TemplateDto> CreateTemplateAsync(CreateTemplateRequest request, Guid authorId);
        Task<TemplateDto> CreateNewVersionAsync(CreateNewVersionRequest request, Guid userId);
        Task<TemplateDto> UpdateTemplateAsync(UpdateTemplateRequest request, Guid userId);
        Task<TemplateDto> PublishTemplateAsync(Guid templateId, Guid userId);
        Task<TemplateDto> ArchiveTemplateAsync(Guid templateId, Guid userId);
        Task DeleteTemplateAsync(Guid templateId, Guid userId);
        Task DeleteAllVersionsAsync(Guid baseTemplateId, Guid userId);

        Task<TemplateDto> GetTemplateByIdAsync(Guid id, Guid? userId = null);
        Task<TemplateDto> GetCurrentVersionAsync(Guid baseTemplateId, Guid? userId = null);
        Task<TemplateDto> GetSpecificVersionAsync(Guid baseTemplateId, int version, Guid? userId = null);
        Task<List<TemplateDto>> GetAllVersionsAsync(Guid baseTemplateId, Guid userId);
        Task<bool> TemplateExistsAsync(Guid id);
        Task<bool> BaseTemplateExistsAsync(Guid baseTemplateId);

        Task<PagedResult<TemplateDto>> GetPublicTemplatesPagedAsync(int page, int pageSize);
        Task<PagedResult<TemplateDto>> GetTemplatesByTagNameAsync(string tagName, int page, int pageSize);
        Task<PagedResult<TemplateDto>> GetPopularTemplatesAsync(int page, int pageSize);
        Task<PagedResult<TemplateDto>> GetTemplatesByAuthorPagedAsync(Guid authorId, int page, int pageSize);
        Task<PagedResult<TemplateDto>> GetUserAccessibleTemplatesPagedAsync(Guid userId, int page, int pageSize);
        Task<List<TemplateDto>> GetLatestTemplatesAsync(int count = 10);

        Task AddTagToTemplateAsync(Guid templateId, string tagName, Guid userId);
        Task RemoveTagFromTemplateAsync(Guid templateId, Guid tagId, Guid userId);
        Task<TemplateDto> UpdateTemplateTagsAsync(Guid templateId, UpdateTemplateTagsRequest request, Guid userId);

        Task<bool> UpdateTemplateImageAsync(Guid templateId, string imageUrl, Guid userId);

        Task AddAllowedUserToTemplateAsync(Guid templateId, Guid allowedUserId, Guid ownerId);
        Task RemoveAllowedUserFromTemplateAsync(Guid templateId, Guid allowedUserId, Guid ownerId);
        Task<List<UserSearchDto>> GetTemplateAllowedUsersAsync(Guid templateId, Guid userId);

        Task<bool> HasUserAccessToTemplateAsync(Guid templateId, Guid userId);
        Task<bool> IsUserTemplateAuthorAsync(Guid templateId, Guid userId);
        Task<bool> IsUserBaseTemplateAuthorAsync(Guid baseTemplateId, Guid userId);
        Task<bool> CanUserEditTemplateAsync(Guid templateId, Guid userId);
        Task<bool> CanUserCreateNewVersionAsync(Guid baseTemplateId, Guid userId);

        Task<string> UploadTemplateImageAsync(Guid templateId, Stream imageStream, string fileName, Guid userId);
        Task RemoveTemplateImageAsync(Guid templateId, Guid userId);

        Task<TemplateVersionInfoDto> GetVersionInfoAsync(Guid baseTemplateId, Guid userId);
    }
}

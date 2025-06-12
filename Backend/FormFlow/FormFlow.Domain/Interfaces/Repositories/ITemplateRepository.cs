﻿using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Interfaces.Repositories
{
    public interface ITemplateRepository
    {
        Task<Template?> GetByIdAsync(Guid id);
        Task<Template?> GetCurrentVersionAsync(Guid baseTemplateId);
        Task<Template?> GetSpecificVersionAsync(Guid baseTemplateId, int version);
        Task<List<Template>> GetAllVersionsAsync(Guid baseTemplateId);
        Task<Template> CreateAsync(Template template);
        Task<Template> CreateNewVersionAsync(Template oldVersion, Template newVersion);
        Task<Template> UpdateAsync(Template template);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> BaseTemplateExistsAsync(Guid baseTemplateId);
        Task DeleteAsync(Guid id);
        Task DeleteAllVersionsAsync(Guid baseTemplateId);

        Task<PagedResult<Template>> GetPublicTemplatesPagedAsync(int page, int pageSize);
        Task<PagedResult<Template>> GetTemplatesByAuthorPagedAsync(Guid authorId, int page, int pageSize);
        Task<PagedResult<Template>> GetUserAccessibleTemplatesPagedAsync(Guid userId, int page, int pageSize);
        Task<List<Template>> GetLatestTemplatesAsync(int count = 10);

        Task<Template?> GetWithQuestionsAsync(Guid id);
        Task<Template?> GetWithFormsAsync(Guid id);
        Task<Template?> GetWithCommentsAsync(Guid id);
        Task<Template?> GetWithAllDetailsAsync(Guid id);

        Task<List<Template>> GetByTagAsync(Guid tagId);
        Task<List<Template>> SearchByTitleOrDescriptionAsync(string query);

        Task<bool> HasUserAccessAsync(Guid templateId, Guid userId);
        Task<bool> IsAuthorAsync(Guid templateId, Guid userId);
        Task<bool> IsAuthorOfBaseTemplateAsync(Guid baseTemplateId, Guid userId);
        Task AddAllowedUserAsync(Guid templateId, Guid userId);
        Task RemoveAllowedUserAsync(Guid templateId, Guid userId);

        Task<int> GetFormsCountAsync(Guid templateId);
        Task<int> GetFormsCountForVersionAsync(Guid templateId, int version);
        Task<int> GetFormsCountForAllVersionsAsync(Guid baseTemplateId);
        Task<int> GetLikesCountAsync(Guid templateId);
        Task<int> GetCommentsCountAsync(Guid templateId);

        Task<int> GetLatestVersionNumberAsync(Guid baseTemplateId);
        Task SetCurrentVersionAsync(Guid baseTemplateId, int version);

        Task AddTagToTemplateAsync(Guid templateId, Guid tagId);
        Task AddTagsToTemplateAsync(Guid templateId, List<Guid> tagIds);
        Task RemoveTagFromTemplateAsync(Guid templateId, Guid tagId);
        Task RemoveAllTagsFromTemplateAsync(Guid templateId);
        Task<List<TemplateTag>> GetTemplateTagsAsync(Guid templateId);
    }
}

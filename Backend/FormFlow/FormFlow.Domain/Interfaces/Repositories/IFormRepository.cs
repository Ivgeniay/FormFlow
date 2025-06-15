using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Interfaces.Repositories
{
    public interface IFormRepository
    {
        Task<Form?> GetByIdAsync(Guid id);
        Task<Form> CreateAsync(Form form);
        Task<Form> UpdateAsync(Form form);
        Task<bool> ExistsAsync(Guid id);
        Task DeleteAsync(Guid id);

        Task<Form?> GetWithTemplateAsync(Guid id);
        Task<Form?> GetWithUserAsync(Guid id);
        Task<Form?> GetWithAllDetailsAsync(Guid id);

        Task<PagedResult<Form>> GetFormsByTemplatePagedAsync(Guid templateId, int page, int pageSize);
        Task<PagedResult<Form>> GetFormsByUserPagedAsync(Guid userId, int page, int pageSize);
        Task<List<Form>> GetUserFormsForAllVersionsAsync(Guid baseTemplateId, Guid userId);

        Task<bool> HasUserSubmittedAsync(Guid templateId, Guid userId);
        Task<bool> CanUserEditAsync(Guid formId, Guid userId);
        Task<bool> CanUserViewAsync(Guid formId, Guid userId);

        Task<int> GetCountByTemplateAsync(Guid templateId);
        Task<int> GetCountByUserAsync(Guid userId);

        Task<Form?> GetUserFormForTemplateAsync(Guid templateId, Guid userId);
        Task<Dictionary<Guid, int>> GetFormsCountByTemplatesAsync(List<Guid> templateIds);
        Task<int> GetTotalFormsCountAsync();
        Task<Dictionary<string, int>> GetFormsCountByMonthAsync();
        Task<List<Guid>> GetMostActiveUsersAsync(int count);
    }
}

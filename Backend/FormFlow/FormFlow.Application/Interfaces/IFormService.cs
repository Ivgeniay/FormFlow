using FormFlow.Application.DTOs.Forms;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Interfaces
{
    public interface IFormService
    {
        Task<FormDto> SubmitFormAsync(SubmitFormRequest request, Guid userId);
        Task<FormDto> UpdateFormAsync(UpdateFormRequest request, Guid userId);
        Task DeleteFormAsync(Guid formId, Guid userId);
        Task<FormDto> GetFormByIdAsync(Guid formId, Guid userId);
        Task<bool> FormExistsAsync(Guid formId);

        Task<PagedResult<FormDto>> GetFormsByTemplatePagedAsync(Guid templateId, int page, int pageSize, Guid userId);
        Task<PagedResult<FormDto>> GetFormsByUserPagedAsync(Guid userId, int page, int pageSize);
        Task<List<FormDto>> GetUserFormsForAllVersionsAsync(Guid baseTemplateId, Guid userId);

        Task<FormDto?> GetUserFormForTemplateAsync(Guid templateId, Guid userId);
        Task<bool> HasUserSubmittedFormAsync(Guid templateId, Guid userId);
        Task<bool> CanUserViewFormAsync(Guid formId, Guid userId);
        Task<bool> CanUserEditFormAsync(Guid formId, Guid userId);

        Task<FormAccessDto> GetFormAccessAsync(Guid templateId, Guid userId);
        Task<PagedResult<FormDto>> GetAllForAdmin(Guid userId, int psge, int pageSize);
    }
}

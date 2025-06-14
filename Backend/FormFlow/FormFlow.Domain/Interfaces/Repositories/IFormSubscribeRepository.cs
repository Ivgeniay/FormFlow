using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Interfaces.Repositories
{
    public interface IFormSubscribeRepository
    {
        Task<FormSubscribe> AddAsync(FormSubscribe formSubscribe);
        Task<bool> DeleteAsync(Guid userId, Guid templateId);
        Task<bool> ExistAsync(Guid userId, Guid templateId);
        Task<bool> HasTemplateSubscribersAsync(Guid templateId);
        Task<bool> HasAnySubscriptionsAsync(Guid userId);
        Task<List<User>> GetByTemplateIdAsync(Guid templateId);
        Task<List<Template>> GetByUserIdAsync(Guid userId);
    }
}

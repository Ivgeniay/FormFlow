using FormFlow.Application.DTOs.Templates;
using FormFlow.Application.DTOs.Users;

namespace FormFlow.Application.Interfaces
{
    public interface IFormSubscribeService
    {
        Task<bool> SubscribeAsync(Guid userId, Guid templateId);
        Task<bool> UnsubscribeAsync(Guid userId, Guid templateId);
        Task<bool> IsSubscribedAsync(Guid userId, Guid templateId);
        Task<List<TemplateListItemDto>> GetUserSubscriptionsAsync(Guid userId);
        Task<List<UserSearchDto>> GetSubscribersAsync(Guid templateId);
        Task NotifySubscribersAsync(Guid formId);
        Task NotifyMeAsync(Guid formId, Guid userId);
    }
}

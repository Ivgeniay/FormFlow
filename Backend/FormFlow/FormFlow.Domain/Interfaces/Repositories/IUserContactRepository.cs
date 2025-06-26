using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Interfaces.Repositories
{
    public interface IUserContactRepository
    {
        Task<UserContact?> GetByIdAsync(Guid id);
        Task<List<UserContact>> GetByUserIdAsync(Guid userId);
        Task<UserContact?> GetPrimaryContactAsync(Guid userId);
        Task<UserContact?> GetByValueAsync(string value, ContactType type);
        Task<UserContact> CreateAsync(UserContact contact);
        Task<UserContact> UpdateAsync(UserContact contact);
        Task DeleteAsync(Guid id);
        Task DeleteByUserIdAsync(Guid userId);
        Task<bool> ExistsAsync(Guid userId, string value, ContactType type);
    }
}

using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Interfaces.Repositories
{
    public interface IUserSettingsRepository
    {
        Task<UserSettings?> GetByIdAsync(Guid id);
        Task<UserSettings?> GetByUserIdAsync(Guid userId);
        Task<UserSettings> CreateAsync(UserSettings userSettings);
        Task<UserSettings> UpdateAsync(UserSettings userSettings);
        Task DeleteAsync(Guid id);
        Task DeleteByUserIdAsync(Guid userId);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByUserIdAsync(Guid userId);
        Task<UserSettings> CreateDefaultForUserAsync(Guid userId);
    }
}

using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Interfaces
{
    public interface IUserSettingsService
    {
        Task<UserSettings> CreateDefaultForUserAsync(Guid userId);
        Task<UserSettings> SetColorThemeAsync(Guid userId, Guid colorThemeId);
        Task<UserSettings> SetLanguageAsync(Guid userId, Guid languageId);
        Task<UserSettings?> GetByUserIdAsync(Guid userId);
        Task<bool> ExistsByUserIdAsync(Guid userId);
        Task DeleteByUserIdAsync(Guid userId);
    }
}

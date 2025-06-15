using FormFlow.Application.Interfaces;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Services
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly IColorThemeRepository _colorThemeRepository;
        private readonly ILanguageRepository _languageRepository;

        public UserSettingsService(
            IUserSettingsRepository userSettingsRepository,
            IColorThemeRepository colorThemeRepository,
            ILanguageRepository languageRepository)
        {
            _userSettingsRepository = userSettingsRepository;
            _colorThemeRepository = colorThemeRepository;
            _languageRepository = languageRepository;
        }

        public async Task<UserSettings> CreateDefaultForUserAsync(Guid userId)
        {
            if (await _userSettingsRepository.ExistsByUserIdAsync(userId))
                throw new ArgumentException($"User settings for user '{userId}' already exist");

            return await _userSettingsRepository.CreateDefaultForUserAsync(userId);
        }

        public async Task<UserSettings> SetColorThemeAsync(Guid userId, Guid colorThemeId)
        {
            if (!await _colorThemeRepository.ExistsAsync(colorThemeId))
                throw new ArgumentException($"Color theme with ID '{colorThemeId}' not found");

            var colorTheme = await _colorThemeRepository.GetByIdAsync(colorThemeId);
            if (colorTheme == null || !colorTheme.IsActive)
                throw new ArgumentException("Cannot set inactive color theme");

            var userSettings = await _userSettingsRepository.GetByUserIdAsync(userId);
            if (userSettings == null)
            {
                userSettings = await CreateDefaultForUserAsync(userId);
            }

            userSettings.ColorThemeId = colorThemeId;
            return await _userSettingsRepository.UpdateAsync(userSettings);
        }

        public async Task<UserSettings> SetLanguageAsync(Guid userId, Guid languageId)
        {
            if (!await _languageRepository.ExistsAsync(languageId))
                throw new ArgumentException($"Language with ID '{languageId}' not found");

            var language = await _languageRepository.GetByIdAsync(languageId);
            if (language == null || !language.IsActive)
                throw new ArgumentException("Cannot set inactive language");

            var userSettings = await _userSettingsRepository.GetByUserIdAsync(userId);
            if (userSettings == null)
            {
                userSettings = await CreateDefaultForUserAsync(userId);
            }

            userSettings.LanguageId = languageId;
            return await _userSettingsRepository.UpdateAsync(userSettings);
        }

        public async Task<UserSettings?> GetByUserIdAsync(Guid userId)
        {
            return await _userSettingsRepository.GetByUserIdAsync(userId);
        }

        public async Task<bool> ExistsByUserIdAsync(Guid userId)
        {
            return await _userSettingsRepository.ExistsByUserIdAsync(userId);
        }

        public async Task DeleteByUserIdAsync(Guid userId)
        {
            await _userSettingsRepository.DeleteByUserIdAsync(userId);
        }
    }
}

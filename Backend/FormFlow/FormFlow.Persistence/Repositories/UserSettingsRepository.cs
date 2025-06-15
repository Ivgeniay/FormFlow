using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Repositories
{
    public class UserSettingsRepository : IUserSettingsRepository
    {
        private readonly ApplicationDbContext _context;

        public UserSettingsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserSettings?> GetByIdAsync(Guid id) =>
             await _context.UserSettings
                .Include(us => us.User)
                .Include(us => us.ColorTheme)
                .Include(us => us.Language)
                .FirstOrDefaultAsync(us => us.Id == id);
        

        public async Task<UserSettings?> GetByUserIdAsync(Guid userId) =>
             await _context.UserSettings
                .Include(us => us.User)
                .Include(us => us.ColorTheme)
                .Include(us => us.Language)
                .FirstOrDefaultAsync(us => us.UserId == userId);
        

        public async Task<UserSettings> CreateAsync(UserSettings userSettings)
        {
            _context.UserSettings.Add(userSettings);
            await _context.SaveChangesAsync();
            return userSettings;
        }

        public async Task<UserSettings> UpdateAsync(UserSettings userSettings)
        {
            userSettings.UpdatedAt = DateTime.UtcNow;
            _context.UserSettings.Update(userSettings);
            await _context.SaveChangesAsync();
            return userSettings;
        }

        public async Task DeleteAsync(Guid id)
        {
            var userSettings = await _context.UserSettings.FindAsync(id);
            if (userSettings != null)
            {
                _context.UserSettings.Remove(userSettings);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteByUserIdAsync(Guid userId)
        {
            var userSettings = await _context.UserSettings
                .FirstOrDefaultAsync(us => us.UserId == userId);
            if (userSettings != null)
            {
                _context.UserSettings.Remove(userSettings);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.UserSettings.AnyAsync(us => us.Id == id);
        }

        public async Task<bool> ExistsByUserIdAsync(Guid userId) =>
             await _context.UserSettings.AnyAsync(us => us.UserId == userId);
        

        public async Task<UserSettings> CreateDefaultForUserAsync(Guid userId)
        {
            var defaultColorTheme = await _context.ColorThemes
                .FirstOrDefaultAsync(ct => ct.IsDefault && ct.IsActive);
            var defaultLanguage = await _context.Languages
                .FirstOrDefaultAsync(l => l.IsDefault && l.IsActive);

            if (defaultColorTheme == null)
                throw new InvalidOperationException("No default color theme found");
            if (defaultLanguage == null)
                throw new InvalidOperationException("No default language found");

            var userSettings = new UserSettings
            {
                UserId = userId,
                ColorThemeId = defaultColorTheme.Id,
                LanguageId = defaultLanguage.Id
            };

            return await CreateAsync(userSettings);
        }
    }
}

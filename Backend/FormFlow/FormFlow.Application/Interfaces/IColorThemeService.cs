using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Interfaces
{
    public interface IColorThemeService
    {
        Task<ColorTheme> CreateAsync(ColorTheme colorTheme);
        Task<ColorTheme> UpdateAsync(Guid id, ColorTheme colorTheme);
        Task DeleteAsync(Guid id);
        Task<ColorTheme?> GetByIdAsync(Guid id);
        Task<List<ColorTheme>> GetAllAsync();
        Task<List<ColorTheme>> GetActiveAsync();
        Task<ColorTheme?> GetDefaultAsync();
        Task<ColorTheme> SetDefaultAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}

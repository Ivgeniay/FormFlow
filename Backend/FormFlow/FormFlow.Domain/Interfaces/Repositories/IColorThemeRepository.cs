using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Interfaces.Repositories
{
    public interface IColorThemeRepository
    {
        Task<ColorTheme?> GetByIdAsync(Guid id);
        Task<ColorTheme?> GetByNameAsync(string name);
        Task<ColorTheme?> GetByCssClassAsync(string cssClass);
        Task<ColorTheme?> GetDefaultAsync();
        Task<List<ColorTheme>> GetAllAsync();
        Task<List<ColorTheme>> GetActiveAsync();
        Task<ColorTheme> CreateAsync(ColorTheme colorTheme);
        Task<ColorTheme> UpdateAsync(ColorTheme colorTheme);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> NameExistsAsync(string name);
        Task<bool> CssClassExistsAsync(string cssClass);
        Task<ColorTheme> SetAsDefaultAsync(Guid id);
        Task<int> GetCountAsync();
    }
}

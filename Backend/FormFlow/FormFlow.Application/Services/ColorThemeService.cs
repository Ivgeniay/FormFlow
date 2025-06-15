using FormFlow.Application.Interfaces;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Services
{
    public class ColorThemeService : IColorThemeService
    {
        private readonly IColorThemeRepository _colorThemeRepository;

        public ColorThemeService(IColorThemeRepository colorThemeRepository)
        {
            _colorThemeRepository = colorThemeRepository;
        }

        public async Task<ColorTheme> CreateAsync(ColorTheme colorTheme)
        {
            if (await _colorThemeRepository.NameExistsAsync(colorTheme.Name))
                throw new ArgumentException($"Color theme with name '{colorTheme.Name}' already exists");

            if (await _colorThemeRepository.CssClassExistsAsync(colorTheme.CssClass))
                throw new ArgumentException($"Color theme with CSS class '{colorTheme.CssClass}' already exists");

            var count = await _colorThemeRepository.GetCountAsync();
            colorTheme.IsDefault = count == 0;

            return await _colorThemeRepository.CreateAsync(colorTheme);
        }

        public async Task<ColorTheme> UpdateAsync(Guid id, ColorTheme colorTheme)
        {
            var existing = await _colorThemeRepository.GetByIdAsync(id);
            if (existing == null)
                throw new ArgumentException($"Color theme with ID '{id}' not found");

            var nameExists = await _colorThemeRepository.NameExistsAsync(colorTheme.Name);
            if (nameExists && existing.Name != colorTheme.Name)
                throw new ArgumentException($"Color theme with name '{colorTheme.Name}' already exists");

            var cssClassExists = await _colorThemeRepository.CssClassExistsAsync(colorTheme.CssClass);
            if (cssClassExists && existing.CssClass != colorTheme.CssClass)
                throw new ArgumentException($"Color theme with CSS class '{colorTheme.CssClass}' already exists");

            existing.Name = colorTheme.Name;
            existing.CssClass = colorTheme.CssClass;
            existing.PrimaryColor = colorTheme.PrimaryColor;
            existing.IsActive = colorTheme.IsActive;

            return await _colorThemeRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var colorTheme = await _colorThemeRepository.GetByIdAsync(id);
            if (colorTheme == null)
                throw new ArgumentException($"Color theme with ID '{id}' not found");

            if (colorTheme.IsDefault)
            {
                var count = await _colorThemeRepository.GetCountAsync();
                if (count <= 1)
                    throw new ArgumentException("Cannot delete the last color theme");

                var alternatives = await _colorThemeRepository.GetActiveAsync();
                var alternative = alternatives.FirstOrDefault(ct => ct.Id != id);
                if (alternative != null)
                {
                    await _colorThemeRepository.SetAsDefaultAsync(alternative.Id);
                }
            }

            await _colorThemeRepository.DeleteAsync(id);
        }

        public async Task<ColorTheme?> GetByIdAsync(Guid id) =>
             await _colorThemeRepository.GetByIdAsync(id);

        public async Task<List<ColorTheme>> GetAllAsync() =>
             await _colorThemeRepository.GetAllAsync();

        public async Task<List<ColorTheme>> GetActiveAsync() =>
             await _colorThemeRepository.GetActiveAsync();

        public async Task<ColorTheme?> GetDefaultAsync() =>
             await _colorThemeRepository.GetDefaultAsync();

        public async Task<ColorTheme> SetDefaultAsync(Guid id)
        {
            var colorTheme = await _colorThemeRepository.GetByIdAsync(id);
            if (colorTheme == null)
                throw new ArgumentException($"Color theme with ID '{id}' not found");

            if (!colorTheme.IsActive)
                throw new ArgumentException("Cannot set inactive color theme as default");

            return await _colorThemeRepository.SetAsDefaultAsync(id);
        }

        public async Task<bool> ExistsAsync(Guid id) =>
            await _colorThemeRepository.ExistsAsync(id);
    }
}

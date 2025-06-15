using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Repositories
{
    public class ColorThemeRepository : IColorThemeRepository
    {
        private readonly ApplicationDbContext _context;

        public ColorThemeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ColorTheme?> GetByIdAsync(Guid id) =>
            await _context.ColorThemes.FindAsync(id);

        public async Task<ColorTheme?> GetByNameAsync(string name) =>
            await _context.ColorThemes
                .FirstOrDefaultAsync(ct => ct.Name == name);

        public async Task<ColorTheme?> GetByCssClassAsync(string cssClass) =>
            await _context.ColorThemes
                .FirstOrDefaultAsync(ct => ct.CssClass == cssClass);

        public async Task<ColorTheme?> GetDefaultAsync() =>
            await _context.ColorThemes
                .FirstOrDefaultAsync(ct => ct.IsDefault && ct.IsActive);

        public async Task<List<ColorTheme>> GetAllAsync() =>
            await _context.ColorThemes
                .OrderBy(ct => ct.Name)
                .ToListAsync();

        public async Task<List<ColorTheme>> GetActiveAsync() =>
            await _context.ColorThemes
                .Where(ct => ct.IsActive)
                .OrderBy(ct => ct.Name)
                .ToListAsync();

        public async Task<ColorTheme> CreateAsync(ColorTheme colorTheme)
        {
            _context.ColorThemes.Add(colorTheme);
            await _context.SaveChangesAsync();
            return colorTheme;
        }

        public async Task<ColorTheme> UpdateAsync(ColorTheme colorTheme)
        {
            _context.ColorThemes.Update(colorTheme);
            await _context.SaveChangesAsync();
            return colorTheme;
        }

        public async Task DeleteAsync(Guid id)
        {
            var colorTheme = await _context.ColorThemes.FindAsync(id);
            if (colorTheme != null)
            {
                _context.ColorThemes.Remove(colorTheme);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.ColorThemes.AnyAsync(ct => ct.Id == id);

        public async Task<bool> NameExistsAsync(string name) =>
            await _context.ColorThemes.AnyAsync(ct => ct.Name == name);

        public async Task<bool> CssClassExistsAsync(string cssClass) =>
            await _context.ColorThemes.AnyAsync(ct => ct.CssClass == cssClass);

        public async Task<ColorTheme> SetAsDefaultAsync(Guid id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.ColorThemes
                    .Where(ct => ct.IsDefault)
                    .ExecuteUpdateAsync(ct => ct.SetProperty(x => x.IsDefault, false));

                var colorTheme = await _context.ColorThemes.FindAsync(id);
                if (colorTheme == null)
                    throw new ArgumentException($"Color theme with ID '{id}' not found");

                colorTheme.IsDefault = true;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return colorTheme;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> GetCountAsync() =>
            await _context.ColorThemes.CountAsync();
    }

}

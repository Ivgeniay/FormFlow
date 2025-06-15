using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Repositories
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly ApplicationDbContext _context;

        public LanguageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Language?> GetByIdAsync(Guid id) =>
            await _context.Languages.FindAsync(id);
        

        public async Task<Language?> GetByCodeAsync(string code) =>
             await _context.Languages
                .FirstOrDefaultAsync(l => l.Code == code);

        public async Task<Language?> GetByShortCodeAsync(string shortCode) =>
             await _context.Languages
                .FirstOrDefaultAsync(l => l.ShortCode == shortCode);

        public async Task<Language?> GetByNameAsync(string name) =>
             await _context.Languages
                .FirstOrDefaultAsync(l => l.Name == name);
        

        public async Task<Language?> GetDefaultAsync() =>
             await _context.Languages
                .FirstOrDefaultAsync(l => l.IsDefault && l.IsActive);

        public async Task<List<Language>> GetAllAsync() =>
             await _context.Languages
                .OrderBy(l => l.Name)
                .ToListAsync();
        

        public async Task<List<Language>> GetActiveAsync() =>
             await _context.Languages
                .Where(l => l.IsActive)
                .OrderBy(l => l.Name)
                .ToListAsync();
        

        public async Task<Language> CreateAsync(Language language)
        {
            _context.Languages.Add(language);
            await _context.SaveChangesAsync();
            return language;
        }

        public async Task<Language> UpdateAsync(Language language)
        {
            _context.Languages.Update(language);
            await _context.SaveChangesAsync();
            return language;
        }

        public async Task DeleteAsync(Guid id)
        {
            var language = await _context.Languages.FindAsync(id);
            if (language != null)
            {
                _context.Languages.Remove(language);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id) =>
             await _context.Languages.AnyAsync(l => l.Id == id);
        
        public async Task<bool> CodeExistsAsync(string code) =>
             await _context.Languages.AnyAsync(l => l.Code == code);

        public async Task<bool> NameExistsAsync(string name) =>
             await _context.Languages.AnyAsync(l => l.Name == name);
    }
}

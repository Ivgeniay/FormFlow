using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Interfaces.Repositories
{
    public interface ILanguageRepository
    {
        Task<Language?> GetByIdAsync(Guid id);
        Task<Language?> GetByCodeAsync(string code);
        Task<Language?> GetByShortCodeAsync(string shortCode);
        Task<Language?> GetByNameAsync(string name);
        Task<Language?> GetDefaultAsync();
        Task<List<Language>> GetAllAsync();
        Task<List<Language>> GetActiveAsync();
        Task<Language> CreateAsync(Language language);
        Task<Language> UpdateAsync(Language language);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> CodeExistsAsync(string code);
        Task<bool> NameExistsAsync(string name);
        Task<Language> SetAsDefaultAsync(Guid id);
        Task<int> GetCountAsync();
    }
}

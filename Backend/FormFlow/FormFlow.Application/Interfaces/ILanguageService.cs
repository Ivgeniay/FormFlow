using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Interfaces
{
    public interface ILanguageService
    {
        Task<Language> CreateAsync(Language language);
        Task<Language> UpdateAsync(Guid id, Language language);
        Task DeleteAsync(Guid id);
        Task<Language?> GetByIdAsync(Guid id);
        Task<List<Language>> GetAllAsync();
        Task<List<Language>> GetActiveAsync();
        Task<Language?> GetDefaultAsync();
        Task<Language> SetDefaultAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}

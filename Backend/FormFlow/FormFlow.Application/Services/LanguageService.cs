using FormFlow.Application.Interfaces;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _languageRepository;

        public LanguageService(ILanguageRepository languageRepository)
        {
            _languageRepository = languageRepository;
        }

        public async Task<Language> CreateAsync(Language language)
        {
            if (await _languageRepository.CodeExistsAsync(language.Code))
                throw new ArgumentException($"Language with code '{language.Code}' already exists");

            if (await _languageRepository.NameExistsAsync(language.Name))
                throw new ArgumentException($"Language with name '{language.Name}' already exists");

            var count = await _languageRepository.GetCountAsync();
            language.IsDefault = count == 0;

            return await _languageRepository.CreateAsync(language);
        }

        public async Task<Language> UpdateAsync(Guid id, Language language)
        {
            var existing = await _languageRepository.GetByIdAsync(id);
            if (existing == null)
                throw new ArgumentException($"Language with ID '{id}' not found");

            var codeExists = await _languageRepository.CodeExistsAsync(language.Code);
            if (codeExists && existing.Code != language.Code)
                throw new ArgumentException($"Language with code '{language.Code}' already exists");

            var nameExists = await _languageRepository.NameExistsAsync(language.Name);
            if (nameExists && existing.Name != language.Name)
                throw new ArgumentException($"Language with name '{language.Name}' already exists");

            existing.Code = language.Code;
            existing.ShortCode = language.ShortCode;
            existing.Name = language.Name;
            existing.Region = language.Region;
            existing.IconURL = language.IconURL;
            existing.IsActive = language.IsActive;

            return await _languageRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var language = await _languageRepository.GetByIdAsync(id);
            if (language == null)
                throw new ArgumentException($"Language with ID '{id}' not found");

            if (language.IsDefault)
            {
                var count = await _languageRepository.GetCountAsync();
                if (count <= 1)
                    throw new ArgumentException("Cannot delete the last language");

                var alternatives = await _languageRepository.GetActiveAsync();
                var alternative = alternatives.FirstOrDefault(l => l.Id != id);
                if (alternative != null)
                {
                    await _languageRepository.SetAsDefaultAsync(alternative.Id);
                }
            }

            await _languageRepository.DeleteAsync(id);
        }

        public async Task<Language?> GetByIdAsync(Guid id) =>
             await _languageRepository.GetByIdAsync(id);

        public async Task<List<Language>> GetAllAsync()
        {
            return await _languageRepository.GetAllAsync();
        }

        public async Task<List<Language>> GetActiveAsync() =>
             await _languageRepository.GetActiveAsync();

        public async Task<Language?> GetDefaultAsync() =>
             await _languageRepository.GetDefaultAsync();

        public async Task<Language> SetDefaultAsync(Guid id)
        {
            var language = await _languageRepository.GetByIdAsync(id);
            if (language == null)
                throw new ArgumentException($"Language with ID '{id}' not found");

            if (!language.IsActive)
                throw new ArgumentException("Cannot set inactive language as default");

            return await _languageRepository.SetAsDefaultAsync(id);
        }

        public async Task<bool> ExistsAsync(Guid id) =>
            await _languageRepository.ExistsAsync(id);
        
    }
}

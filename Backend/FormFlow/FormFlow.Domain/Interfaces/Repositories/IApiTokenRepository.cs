using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Interfaces.Repositories
{
    public interface IApiTokenRepository
    {
        Task<ApiToken?> GetByIdAsync(Guid id);
        Task<ApiToken?> GetByTokenHashAsync(string tokenHash);
        Task<ApiToken?> GetActiveByUserIdAsync(Guid userId);
        Task<List<ApiToken>> GetByUserIdAsync(Guid userId);
        Task<ApiToken> CreateAsync(ApiToken apiToken);
        Task<ApiToken> UpdateAsync(ApiToken apiToken);
        Task DeleteAsync(Guid id);
        Task DeactivateUserTokensAsync(Guid userId);
        Task UpdateLastUsedAsync(Guid tokenId, DateTime lastUsedAt);
        Task<bool> IsTokenValidAsync(string tokenHash);
    }
}
using FormFlow.Domain.Models.DTOs;

namespace FormFlow.Domain.Interfaces.Services
{
    public interface IApiTokenService
    {
        Task<ApiTokenDto> GenerateTokenAsync(Guid userId);
        Task<ApiTokenDto?> GetActiveTokenAsync(Guid userId);
        Task<List<ApiTokenDto>> GetUserTokensAsync(Guid userId);
        Task RevokeTokenAsync(Guid tokenId, Guid userId);
        Task RevokeAllUserTokensAsync(Guid userId);
        Task<bool> ValidateTokenAsync(string token);
        Task<Guid?> GetUserIdByTokenAsync(string token);
        Task UpdateTokenLastUsedAsync(string token);
    }
}
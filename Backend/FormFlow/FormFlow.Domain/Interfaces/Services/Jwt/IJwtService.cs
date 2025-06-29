using FormFlow.Domain.Models.General;
using FormFlow.Domain.Models.Auth;

namespace FormFlow.Domain.Interfaces.Services.Jwt
{
    public interface IJwtService
    {
        Task<JwtTokenResult> GenerateTokenAsync(User user, AuthType authType);
        Task<JwtTokenResult> RefreshTokenAsync(string refreshToken);
        Task<bool> ValidateTokenAsync(string token);
        Task<JwtClaimsPrincipal?> GetClaimsFromTokenAsync(string token);
        Task RevokeTokenAsync(string refreshToken);
        Task RevokeAllUserTokensAsync(Guid userId);
        Task<JwtTokenResult> RegenerateTokenForRoleChangeAsync(Guid userId, AuthType authType);
        Task<bool> IsRefreshTokenValidAsync(string refreshToken, Guid userId);
        Task<JwtTokenResult> GenerateTokenFromExternalAuthAsync(User user, ExternalAuthInfo externalAuth);
        Task UpdateExternalRefreshTokenAsync(Guid userId, string externalRefreshToken, DateTime externalTokenExpiry);
    }

}

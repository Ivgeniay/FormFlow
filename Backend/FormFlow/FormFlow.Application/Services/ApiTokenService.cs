using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Interfaces.Services;
using FormFlow.Domain.Models.General;
using FormFlow.Domain.Models.DTOs;
using FormFlow.Domain.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace FormFlow.Application.Services
{
    public class ApiTokenService : IApiTokenService
    {
        private readonly IApiTokenRepository _apiTokenRepository;
        private readonly IUserRepository _userRepository;

        public ApiTokenService(IApiTokenRepository apiTokenRepository, IUserRepository userRepository)
        {
            _apiTokenRepository = apiTokenRepository;
            _userRepository = userRepository;
        }

        public async Task<ApiTokenDto> GenerateTokenAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            await _apiTokenRepository.DeactivateUserTokensAsync(userId);

            var plainToken = GenerateSecureToken();
            var tokenHash = HashToken(plainToken);

            var apiToken = new ApiToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TokenHash = tokenHash,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _apiTokenRepository.CreateAsync(apiToken);

            return new ApiTokenDto
            {
                Id = apiToken.Id,
                Token = plainToken,
                CreatedAt = apiToken.CreatedAt,
                IsActive = apiToken.IsActive
            };
        }

        public async Task<ApiTokenDto?> GetActiveTokenAsync(Guid userId)
        {
            var token = await _apiTokenRepository.GetActiveByUserIdAsync(userId);
            if (token == null)
                return null;

            return new ApiTokenDto
            {
                Id = token.Id,
                Token = "***",
                CreatedAt = token.CreatedAt,
                IsActive = token.IsActive
            };
        }

        public async Task<List<ApiTokenDto>> GetUserTokensAsync(Guid userId)
        {
            var tokens = await _apiTokenRepository.GetByUserIdAsync(userId);
            return tokens.Select(t => new ApiTokenDto
            {
                Id = t.Id,
                Token = "***",
                CreatedAt = t.CreatedAt,
                IsActive = t.IsActive
            }).ToList();
        }

        public async Task RevokeTokenAsync(Guid tokenId, Guid userId)
        {
            var token = await _apiTokenRepository.GetByIdAsync(tokenId);
            if (token == null)
                throw new ApiTokenNotFoundException(tokenId);

            if (token.UserId != userId)
                throw new ApiTokenAccessDeniedException();

            token.IsActive = false;
            await _apiTokenRepository.UpdateAsync(token);
        }

        public async Task RevokeAllUserTokensAsync(Guid userId)
        {
            await _apiTokenRepository.DeactivateUserTokensAsync(userId);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            var tokenHash = HashToken(token);
            return await _apiTokenRepository.IsTokenValidAsync(tokenHash);
        }

        public async Task<Guid?> GetUserIdByTokenAsync(string token)
        {
            var tokenHash = HashToken(token);
            var apiToken = await _apiTokenRepository.GetByTokenHashAsync(tokenHash);
            return apiToken?.UserId;
        }

        public async Task UpdateTokenLastUsedAsync(string token)
        {
            var tokenHash = HashToken(token);
            var apiToken = await _apiTokenRepository.GetByTokenHashAsync(tokenHash);
            if (apiToken != null)
            {
                await _apiTokenRepository.UpdateLastUsedAsync(apiToken.Id, DateTime.UtcNow);
            }
        }

        private string GenerateSecureToken()
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        private string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
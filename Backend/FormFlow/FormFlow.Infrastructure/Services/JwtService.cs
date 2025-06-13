using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using FormFlow.Domain.Models.General;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using FormFlow.Domain.Models.Auth;
using System.Security.Claims;
using System.Text;
using static FormFlow.Infrastructure.Constants;

namespace FormFlow.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _accessTokenExpiryMinutes;
        private readonly int _refreshTokenExpiryDays;

        public JwtService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _secretKey = _configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException("Jwt:SecretKey");
            _issuer = _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer");
            _audience = _configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience");
            _accessTokenExpiryMinutes = int.Parse(_configuration["Jwt:AccessTokenExpiryMinutes"] ?? "15");
            _refreshTokenExpiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7");
        }

        public async Task<JwtTokenResult> GenerateTokenAsync(User user, AuthType authType)
        {
            var accessToken = GenerateAccessToken(user, authType);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            await SaveRefreshTokenAsync(user, refreshToken, refreshTokenExpiry, authType);

            return new JwtTokenResult
            {
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken,
                AccessTokenExpiry = accessToken.Expiry,
                RefreshTokenExpiry = refreshTokenExpiry,
                UserId = user.Id,
                UserRole = user.Role,
                AuthType = authType
            };
        }

        public async Task<JwtTokenResult> GenerateTokenFromExternalAuthAsync(User user, ExternalAuthInfo externalAuth)
        {
            var accessToken = GenerateAccessToken(user, externalAuth.Provider);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            await SaveRefreshTokenAsync(user, refreshToken, refreshTokenExpiry, externalAuth.Provider);
            await UpdateExternalRefreshTokenAsync(user.Id, externalAuth.ExternalRefreshToken, externalAuth.ExternalTokenExpiry);

            return new JwtTokenResult
            {
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken,
                AccessTokenExpiry = accessToken.Expiry,
                RefreshTokenExpiry = refreshTokenExpiry,
                UserId = user.Id,
                UserRole = user.Role,
                AuthType = externalAuth.Provider
            };
        }

        public async Task<JwtTokenResult> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid refresh token");

            if (!IsRefreshTokenValidInternal(user, refreshToken))
                throw new UnauthorizedAccessException("Refresh token is expired or revoked");

            var authType = DetermineAuthTypeFromRefreshToken(user, refreshToken);
            await RevokeRefreshTokenAsync(user, refreshToken);

            return await GenerateTokenAsync(user, authType);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<JwtClaimsPrincipal?> GetClaimsFromTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtClaimNames.Subject);
                var userNameClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtClaimNames.Name);
                var emailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtClaimNames.Email);
                var roleClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtClaimNames.Role);
                var authTypeClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtClaimNames.AuthType);
                var isBlockedClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtClaimNames.IsBlocked);

                if (userIdClaim == null || userNameClaim == null || emailClaim == null ||
                    roleClaim == null || authTypeClaim == null || isBlockedClaim == null)
                    return null;

                if (!Guid.TryParse(userIdClaim.Value, out var userId) ||
                    !Enum.TryParse<UserRole>(roleClaim.Value, out var role) ||
                    !Enum.TryParse<AuthType>(authTypeClaim.Value, out var authType) ||
                    !bool.TryParse(isBlockedClaim.Value, out var isBlocked))
                    return null;

                return new JwtClaimsPrincipal
                {
                    UserId = userId,
                    UserName = userNameClaim.Value,
                    Role = role,
                    Email = emailClaim.Value,
                    IsBlocked = isBlocked,
                    AuthType = authType,
                    TokenIssuedAt = jwtToken.ValidFrom,
                    TokenExpiresAt = jwtToken.ValidTo
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
            if (user != null)
            {
                await RevokeRefreshTokenAsync(user, refreshToken);
            }
        }

        public async Task RevokeAllUserTokensAsync(Guid userId)
        {
            var user = await _userRepository.GetWithAuthMethodsAsync(userId);
            if (user == null) return;

            if (user.EmailAuth != null)
            {
                user.EmailAuth.RefreshTokenRevokedAt = DateTime.UtcNow;
            }

            if (user.GoogleAuth != null)
            {
                user.GoogleAuth.RefreshTokenRevokedAt = DateTime.UtcNow;
                user.GoogleAuth.UpdatedAt = DateTime.UtcNow;
            }

            await _userRepository.UpdateAsync(user);
        }

        public async Task<JwtTokenResult> RegenerateTokenForRoleChangeAsync(Guid userId, AuthType authType)
        {
            var user = await _userRepository.GetWithAuthMethodsAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            await RevokeAllUserTokensAsync(userId);
            return await GenerateTokenAsync(user, authType);
        }

        public async Task<bool> IsRefreshTokenValidAsync(string refreshToken, Guid userId)
        {
            var user = await _userRepository.GetWithAuthMethodsAsync(userId);
            if (user == null) return false;

            return IsRefreshTokenValidInternal(user, refreshToken);
        }

        public async Task UpdateExternalRefreshTokenAsync(Guid userId, string externalRefreshToken, DateTime externalTokenExpiry)
        {
            var user = await _userRepository.GetWithAuthMethodsAsync(userId);
            if (user?.GoogleAuth != null)
            {
                user.GoogleAuth.RefreshToken = externalRefreshToken;
                user.GoogleAuth.TokenExpiry = externalTokenExpiry;
                user.GoogleAuth.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
            }
        }

        private (string Token, DateTime Expiry) GenerateAccessToken(User user, AuthType authType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);
            var expiry = DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes);

            var claims = new[]
            {
                new Claim(JwtClaimNames.Subject, user.Id.ToString()),
                new Claim(JwtClaimNames.Name, user.UserName),
                new Claim(JwtClaimNames.Email, GetUserEmail(user, authType)),
                new Claim(JwtClaimNames.Role, user.Role.ToString()),
                new Claim(JwtClaimNames.AuthType, authType.ToString()),
                new Claim(JwtClaimNames.IsBlocked, user.IsBlocked.ToString()),
                new Claim(JwtClaimNames.IssuedAt, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtClaimNames.TokenId, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiry,
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (tokenHandler.WriteToken(token), expiry);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private string GetUserEmail(User user, AuthType authType)
        {
            return authType switch
            {
                AuthType.Internal => user.EmailAuth?.Email ?? "",
                AuthType.Google => user.GoogleAuth?.Email ?? "",
                _ => ""
            };
        }

        private AuthType DetermineAuthTypeFromRefreshToken(User user, string refreshToken)
        {
            if (user.EmailAuth?.RefreshToken == refreshToken)
                return AuthType.Internal;
            if (user.GoogleAuth?.RefreshToken == refreshToken)
                return AuthType.Google;
            return AuthType.Internal;
        }

        private async Task SaveRefreshTokenAsync(User user, string refreshToken, DateTime expiry, AuthType authType)
        {
            if (authType == AuthType.Internal && user.EmailAuth != null)
            {
                user.EmailAuth.RefreshToken = refreshToken;
                user.EmailAuth.RefreshTokenExpiresAt = expiry;
                user.EmailAuth.RefreshTokenRevokedAt = null;
            }
            else if (authType == AuthType.Google && user.GoogleAuth != null)
            {
                user.GoogleAuth.RefreshToken = refreshToken;
                user.GoogleAuth.RefreshTokenExpiresAt = expiry;
                user.GoogleAuth.RefreshTokenRevokedAt = null;
                user.GoogleAuth.UpdatedAt = DateTime.UtcNow;
            }

            await _userRepository.UpdateAsync(user);
        }

        private bool IsRefreshTokenValidInternal(User user, string refreshToken)
        {
            if (user.EmailAuth?.RefreshToken == refreshToken)
            {
                return user.EmailAuth.RefreshTokenRevokedAt == null &&
                       user.EmailAuth.RefreshTokenExpiresAt > DateTime.UtcNow;
            }

            if (user.GoogleAuth?.RefreshToken == refreshToken)
            {
                return user.GoogleAuth.RefreshTokenRevokedAt == null &&
                       user.GoogleAuth.RefreshTokenExpiresAt > DateTime.UtcNow;
            }

            return false;
        }

        private async Task RevokeRefreshTokenAsync(User user, string refreshToken)
        {
            if (user.EmailAuth?.RefreshToken == refreshToken)
            {
                user.EmailAuth.RefreshTokenRevokedAt = DateTime.UtcNow;
            }
            else if (user.GoogleAuth?.RefreshToken == refreshToken)
            {
                user.GoogleAuth.RefreshTokenRevokedAt = DateTime.UtcNow;
                user.GoogleAuth.UpdatedAt = DateTime.UtcNow;
            }

            await _userRepository.UpdateAsync(user);
        }
    }

}

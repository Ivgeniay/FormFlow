using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Models.Auth
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }
        public bool IsRevoked => RevokedAt.HasValue;
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => !IsRevoked && !IsExpired;
        public string? RevokedReason { get; set; }
        public string? ReplacedByToken { get; set; }
        public AuthType TokenType { get; set; } = AuthType.Internal;
        public string? ExternalProvider { get; set; }
        public string? ExternalRefreshToken { get; set; }
        public DateTime? ExternalTokenExpiry { get; set; }
        public string DeviceInfo { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; }

        public void Revoke(string reason = "")
        {
            RevokedAt = DateTime.UtcNow;
            RevokedReason = reason;
        }

        public void ReplaceWith(string newToken, string reason = "Token refreshed")
        {
            Revoke(reason);
            ReplacedByToken = newToken;
        }

        public void UpdateLastUsed()
        {
            LastUsedAt = DateTime.UtcNow;
        }

        public void UpdateExternalToken(string externalRefreshToken, DateTime externalTokenExpiry)
        {
            ExternalRefreshToken = externalRefreshToken;
            ExternalTokenExpiry = externalTokenExpiry;
            LastUsedAt = DateTime.UtcNow;
        }
    }
}

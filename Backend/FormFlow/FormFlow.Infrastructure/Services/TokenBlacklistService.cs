using FormFlow.Domain.Interfaces.Services.Jwt;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace FormFlow.Infrastructure.Services
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly ConcurrentDictionary<Guid, DateTime> _blacklist = new();
        private readonly int _defaultExpiryMinutes;
        private readonly Timer _cleanupTimer;

        public TokenBlacklistService(IConfiguration configuration)
        {
            _defaultExpiryMinutes = int.Parse(configuration["Jwt:AccessTokenExpiryMinutes"] ?? "15");

            _cleanupTimer = new Timer(
                callback: _ => CleanupExpiredEntries(),
                state: null,
                dueTime: TimeSpan.FromMinutes(5),
                period: TimeSpan.FromMinutes(5)
            );
        }

        public void AddToBlacklist(Guid userId, TimeSpan? expiry = null)
        {
            var expiryTime = DateTime.UtcNow.Add(expiry ?? TimeSpan.FromMinutes(_defaultExpiryMinutes));
            _blacklist.AddOrUpdate(userId, expiryTime, (key, oldValue) => expiryTime);
        }

        public void RemoveFromBlacklist(Guid userId)
        {
            _blacklist.TryRemove(userId, out _);
        }

        public bool IsBlacklisted(Guid userId)
        {
            if (_blacklist.TryGetValue(userId, out var expiry))
            {
                if (DateTime.UtcNow < expiry)
                {
                    return true;
                }
                else
                {
                    _blacklist.TryRemove(userId, out _);
                    return false;
                }
            }
            return false;
        }

        public void CleanupExpiredEntries()
        {
            var now = DateTime.UtcNow;
            var expiredKeys = _blacklist
                .Where(kvp => kvp.Value < now)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _blacklist.TryRemove(key, out _);
            }
        }

        public void Dispose()
        {
            _cleanupTimer?.Dispose();
        }
    }
}

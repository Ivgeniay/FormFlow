namespace FormFlow.Domain.Interfaces.Services.Jwt
{
    public interface ITokenBlacklistService
    {
        void AddToBlacklist(Guid userId, TimeSpan? expiry = null);
        void RemoveFromBlacklist(Guid userId);
        bool IsBlacklisted(Guid userId);
        void CleanupExpiredEntries();
    }
}

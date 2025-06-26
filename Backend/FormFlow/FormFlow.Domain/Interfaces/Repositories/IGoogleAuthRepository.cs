using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Interfaces.Repositories
{
    public interface IGoogleAuthRepository
    {
        Task<GoogleAuth?> GetByGoogleIdAsync(string googleId);
        Task<GoogleAuth?> GetByUserIdAsync(Guid userId);
        Task<GoogleAuth?> GetByEmailAsync(string email);
        Task<GoogleAuth> CreateAsync(GoogleAuth googleAuth);
        Task<GoogleAuth> UpdateAsync(GoogleAuth googleAuth);
        Task DeleteAsync(Guid id);
        Task DeleteByUserIdAsync(Guid userId);
        Task<bool> ExistsByGoogleIdAsync(string googleId);
        Task<bool> ExistsByUserIdAsync(Guid userId);
    }
}

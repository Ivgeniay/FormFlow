using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Interfaces.Repositories
{
    public interface IEmailAuthRepository
    {
        Task<EmailPasswordAuth?> GetByEmailAsync(string email);
        Task<EmailPasswordAuth?> GetByUserIdAsync(Guid userId);
        Task<EmailPasswordAuth?> GetByRefreshTokenAsync(string refreshToken);
        Task<EmailPasswordAuth> CreateAsync(EmailPasswordAuth emailAuth);
        Task<EmailPasswordAuth> UpdateAsync(EmailPasswordAuth emailAuth);
        Task DeleteAsync(Guid id);
        Task DeleteByUserIdAsync(Guid userId);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByUserIdAsync(Guid userId);
    }
}

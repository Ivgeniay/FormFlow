using FormFlow.Domain.Models.Analytics;
using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByIdWithContactsAsync(Guid id);
        Task<User?> GetByIdWithAuthAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUserNameAsync(string userName);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UserNameExistsAsync(string userName);
        Task DeleteAsync(Guid id);

        Task<PagedResult<User>> GetUsersPagedAsync(int page, int pageSize);
        Task<PagedResult<User>> GetUsersWithContactsPagedAsync(int page, int pageSize);
        Task<List<User>> SearchByNameOrEmailAsync(string query, int limit = 10);
        Task<List<User>> GetUsersForSearchAsync(string query, int limit = 10);

        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
        Task<User?> GetWithContactsAsync(Guid id);
        Task<User?> GetWithAuthMethodsAsync(Guid id);
        Task<User?> GetForAuthenticationAsync(string email);

        Task<User> CreateUserWithAuthAsync(User user);
        Task<Dictionary<string, int>> GetUsersCountByMonthAsync();
        Task<List<UserAnalyticsStatsDto>> GetUserAnalyticsStatsAsync(List<Guid> userIds);
        Task<List<User>> GetUsersWithContactsAsync(Guid[] ids);
    }
}

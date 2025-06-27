using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.Analytics;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.EmailAuth)
                .Include(u => u.GoogleAuth)
                .FirstOrDefaultAsync(u => !u.IsDeleted && (
                    (u.EmailAuth != null && u.EmailAuth.Email == email) ||
                    (u.GoogleAuth != null && u.GoogleAuth.Email == email)));
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName && !u.IsDeleted);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Users
                .AnyAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.EmailPasswordAuths
                .AnyAsync(e => e.Email == email && !e.User.IsDeleted);
        }

        public async Task<bool> UserNameExistsAsync(string userName)
        {
            return await _context.Users
                .AnyAsync(u => u.UserName == userName && !u.IsDeleted);
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsDeleted = true;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PagedResult<User>> GetUsersPagedAsync(int page, int pageSize)
        {
            var query = _context.Users
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.UserName);

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<User>(users, totalCount, page, pageSize);
        }

        public async Task<PagedResult<User>> GetUsersWithContactsPagedAsync(int page, int pageSize)
        {
            var query = _context.Users
                .Include(u => u.Contacts)
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.UserName);

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<User>(users, totalCount, page, pageSize);
        }

        public async Task<List<User>> SearchByNameOrEmailAsync(string query, int limit = 10)
        {
            var normalizedQuery = query.ToLower();

            return await _context.Users
                .Include(u => u.EmailAuth)
                .Include(u => u.GoogleAuth)
                .Include(u => u.Contacts)
                .Where(u => !u.IsDeleted &&
                    (u.UserName.ToLower().Contains(normalizedQuery) ||
                     (u.EmailAuth != null && u.EmailAuth.Email.ToLower().Contains(normalizedQuery)) ||
                     (u.GoogleAuth != null && u.GoogleAuth.Email.ToLower().Contains(normalizedQuery))))
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersForSearchAsync(string query, int limit = 10)
        {
            var normalizedQuery = query.ToLower();

            return await _context.Users
                .Include(u => u.Contacts.Where(c => c.IsPrimary && c.Type == ContactType.Email))
                .Where(u => !u.IsDeleted &&
                    (u.UserName.ToLower().Contains(normalizedQuery) ||
                     u.Contacts.Any(c => c.IsPrimary && c.Type == ContactType.Email && c.Value.ToLower().Contains(normalizedQuery))))
                .Take(limit)
                .ToListAsync();
        }

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                .Include(u => u.EmailAuth)
                .Include(u => u.GoogleAuth)
                .FirstOrDefaultAsync((u) => !u.IsDeleted &&
                    (u.EmailAuth != null && u.EmailAuth.RefreshToken == refreshToken) ||
                    (u.GoogleAuth != null && u.GoogleAuth.RefreshToken == refreshToken)
                );
        }

        public async Task<User?> GetWithContactsAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.Contacts)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<User?> GetWithAuthMethodsAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.EmailAuth)
                .Include(u => u.GoogleAuth)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<User?> GetForAuthenticationAsync(string email)
        {
            return await _context.Users
                .Include(u => u.EmailAuth)
                .Include(u => u.GoogleAuth)
                .Include(u => u.Contacts)
                .FirstOrDefaultAsync(u => u.EmailAuth != null && u.EmailAuth.Email == email && !u.IsDeleted);
        }

        public async Task<User> CreateUserWithAuthAsync(User user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return user;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<Dictionary<string, int>> GetUsersCountByMonthAsync()
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .GroupBy(u => u.CreatedAt.ToString("yyyy-MM"))
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<List<UserAnalyticsStatsDto>> GetUserAnalyticsStatsAsync(List<Guid> userIds)
        {
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id) && !u.IsDeleted)
                .Select(u => new UserAnalyticsStatsDto
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    TemplatesCount = u.Templates.Count(t => !t.IsDeleted && t.IsPublished),
                    FormsCount = u.Forms.Count(f => !f.IsDeleted),
                    CommentsCount = u.Comments.Count(c => !c.IsDeleted),
                    LikesGivenCount = u.Likes.Count(l => !l.IsDeleted)
                })
                .ToListAsync();

            return users;
        }
    }
}

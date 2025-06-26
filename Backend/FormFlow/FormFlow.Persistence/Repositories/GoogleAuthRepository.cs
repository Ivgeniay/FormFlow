using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Repositories
{
    public class GoogleAuthRepository : IGoogleAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public GoogleAuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GoogleAuth?> GetByGoogleIdAsync(string googleId)
        {
            return await _context.GoogleAuths
                .Include(g => g.User)
                .FirstOrDefaultAsync(g => g.GoogleId == googleId);
        }

        public async Task<GoogleAuth?> GetByUserIdAsync(Guid userId)
        {
            return await _context.GoogleAuths
                .FirstOrDefaultAsync(g => g.UserId == userId);
        }

        public async Task<GoogleAuth?> GetByEmailAsync(string email) =>
            await _context.GoogleAuths.FirstOrDefaultAsync(g => g.Email == email);
        

        public async Task<GoogleAuth> CreateAsync(GoogleAuth googleAuth)
        {
            _context.GoogleAuths.Add(googleAuth);
            await _context.SaveChangesAsync();
            return googleAuth;
        }

        public async Task<GoogleAuth> UpdateAsync(GoogleAuth googleAuth)
        {
            googleAuth.UpdatedAt = DateTime.UtcNow;
            _context.GoogleAuths.Update(googleAuth);
            await _context.SaveChangesAsync();
            return googleAuth;
        }

        public async Task DeleteAsync(Guid id)
        {
            var googleAuth = await _context.GoogleAuths.FindAsync(id);
            if (googleAuth != null)
            {
                _context.GoogleAuths.Remove(googleAuth);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteByUserIdAsync(Guid userId)
        {
            var googleAuth = await GetByUserIdAsync(userId);
            if (googleAuth != null)
            {
                _context.GoogleAuths.Remove(googleAuth);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByGoogleIdAsync(string googleId)
        {
            return await _context.GoogleAuths
                .AnyAsync(g => g.GoogleId == googleId);
        }

        public async Task<bool> ExistsByUserIdAsync(Guid userId)
        {
            return await _context.GoogleAuths
                .AnyAsync(g => g.UserId == userId);
        }
    }
}

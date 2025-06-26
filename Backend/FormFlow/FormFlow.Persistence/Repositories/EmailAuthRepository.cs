using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Repositories
{
    public class EmailAuthRepository : IEmailAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public EmailAuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EmailPasswordAuth?> GetByEmailAsync(string email)
        {
            return await _context.EmailPasswordAuths
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<EmailPasswordAuth?> GetByUserIdAsync(Guid userId)
        {
            return await _context.EmailPasswordAuths
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task<EmailPasswordAuth?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.EmailPasswordAuths
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.RefreshToken == refreshToken);
        }

        public async Task<EmailPasswordAuth> CreateAsync(EmailPasswordAuth emailAuth)
        {
            _context.EmailPasswordAuths.Add(emailAuth);
            await _context.SaveChangesAsync();
            return emailAuth;
        }

        public async Task<EmailPasswordAuth> UpdateAsync(EmailPasswordAuth emailAuth)
        {
            var existingEmailAuth = await _context.EmailPasswordAuths.FindAsync(emailAuth.Id);
            if (existingEmailAuth == null)
                throw new ArgumentException("EmailAuth not found");

            existingEmailAuth.Email = emailAuth.Email;
            existingEmailAuth.PasswordHash = emailAuth.PasswordHash;

            await _context.SaveChangesAsync();
            return existingEmailAuth;
        }

        public async Task DeleteAsync(Guid id)
        {
            var emailAuth = await _context.EmailPasswordAuths.FindAsync(id);
            if (emailAuth != null)
            {
                _context.EmailPasswordAuths.Remove(emailAuth);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteByUserIdAsync(Guid userId)
        {
            var emailAuth = await GetByUserIdAsync(userId);
            if (emailAuth != null)
            {
                _context.EmailPasswordAuths.Remove(emailAuth);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.EmailPasswordAuths
                .AnyAsync(e => e.Email == email);
        }

        public async Task<bool> ExistsByUserIdAsync(Guid userId)
        {
            return await _context.EmailPasswordAuths
                .AnyAsync(e => e.UserId == userId);
        }
    }
}

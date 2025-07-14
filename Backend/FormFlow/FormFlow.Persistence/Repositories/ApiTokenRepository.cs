using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Repositories
{
    public class ApiTokenRepository : IApiTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public ApiTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiToken?> GetByIdAsync(Guid id)
        {
            return await _context.ApiTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ApiToken?> GetByTokenHashAsync(string tokenHash)
        {
            return await _context.ApiTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash && x.IsActive);
        }

        public async Task<ApiToken?> GetActiveByUserIdAsync(Guid userId)
        {
            return await _context.ApiTokens
                .FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive);
        }

        public async Task<List<ApiToken>> GetByUserIdAsync(Guid userId)
        {
            return await _context.ApiTokens
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<ApiToken> CreateAsync(ApiToken apiToken)
        {
            _context.ApiTokens.Add(apiToken);
            await _context.SaveChangesAsync();
            return apiToken;
        }

        public async Task<ApiToken> UpdateAsync(ApiToken apiToken)
        {
            _context.ApiTokens.Update(apiToken);
            await _context.SaveChangesAsync();
            return apiToken;
        }

        public async Task DeleteAsync(Guid id)
        {
            var token = await _context.ApiTokens.FindAsync(id);
            if (token != null)
            {
                _context.ApiTokens.Remove(token);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeactivateUserTokensAsync(Guid userId)
        {
            await _context.ApiTokens
                .Where(x => x.UserId == userId && x.IsActive)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsActive, false));
        }

        public async Task UpdateLastUsedAsync(Guid tokenId, DateTime lastUsedAt)
        {
            await _context.ApiTokens
                .Where(x => x.Id == tokenId)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.CreatedAt, lastUsedAt));
        }

        public async Task<bool> IsTokenValidAsync(string tokenHash)
        {
            return await _context.ApiTokens
                .AnyAsync(x => x.TokenHash == tokenHash && x.IsActive);
        }
    }
}
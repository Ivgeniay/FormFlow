using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly ApplicationDbContext _context;

        public LikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Like?> GetByIdAsync(Guid id)
        {
            return await _context.Likes
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
        }

        public async Task<Like> CreateAsync(Like like)
        {
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
            return like;
        }

        public async Task DeleteAsync(Guid id)
        {
            var like = await _context.Likes.FindAsync(id);
            if (like != null)
            {
                like.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Likes
                .AnyAsync(l => l.Id == id && !l.IsDeleted);
        }

        public async Task<Like?> GetByTemplateAndUserAsync(Guid templateId, Guid userId)
        {
            return await _context.Likes
                .FirstOrDefaultAsync(l => l.TemplateId == templateId && l.UserId == userId && !l.IsDeleted);
        }

        public async Task<PagedResult<Like>> GetLikesByTemplatePagedAsync(Guid templateId, int page, int pageSize)
        {
            var query = _context.Likes
                .Include(l => l.User)
                .Where(l => l.TemplateId == templateId && !l.IsDeleted)
                .OrderByDescending(l => l.CreatedAt);

            var totalCount = await query.CountAsync();
            var likes = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Like>(likes, totalCount, page, pageSize);
        }

        public async Task<PagedResult<Like>> GetLikesByUserPagedAsync(Guid userId, int page, int pageSize)
        {
            var query = _context.Likes
                .Include(l => l.Template)
                .ThenInclude(t => t.Author)
                .Include(l => l.Template.Tags)
                .ThenInclude(tt => tt.Tag)
                .Where(l => l.UserId == userId && !l.IsDeleted)
                .OrderByDescending(l => l.CreatedAt);

            var totalCount = await query.CountAsync();
            var likes = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Like>(likes, totalCount, page, pageSize);
        }

        public async Task<bool> HasUserLikedAsync(Guid templateId, Guid userId)
        {
            return await _context.Likes
                .AnyAsync(l => l.TemplateId == templateId && l.UserId == userId && !l.IsDeleted);
        }

        public async Task<int> GetCountByTemplateAsync(Guid templateId)
        {
            return await _context.Likes
                .CountAsync(l => l.TemplateId == templateId && !l.IsDeleted);
        }

        public async Task<int> GetCountByUserAsync(Guid userId)
        {
            return await _context.Likes
                .CountAsync(l => l.UserId == userId && !l.IsDeleted);
        }

        public async Task<Like> ToggleLikeAsync(Guid templateId, Guid userId)
        {
            var existingLike = await GetByTemplateAndUserAsync(templateId, userId);

            if (existingLike != null)
            {
                await DeleteAsync(existingLike.Id);
                return existingLike;
            }
            else
            {
                var newLike = new Like
                {
                    TemplateId = templateId,
                    UserId = userId
                };
                return await CreateAsync(newLike);
            }
        }

        public async Task<bool> AddLikeAsync(Guid templateId, Guid userId)
        {
            if (await HasUserLikedAsync(templateId, userId))
                return false;

            var like = new Like
            {
                TemplateId = templateId,
                UserId = userId
            };

            await CreateAsync(like);
            return true;
        }

        public async Task<bool> RemoveLikeAsync(Guid templateId, Guid userId)
        {
            var existingLike = await GetByTemplateAndUserAsync(templateId, userId);
            if (existingLike == null)
                return false;

            await DeleteAsync(existingLike.Id);
            return true;
        }

        public async Task<List<Template>> GetMostLikedTemplatesAsync(int count = 10)
        {
            var templateIds = await _context.Likes
                .Where(l => !l.IsDeleted)
                .GroupBy(l => l.TemplateId)
                .OrderByDescending(g => g.Count())
                .Take(count)
                .Select(g => g.Key)
                .ToListAsync();

            return await _context.Templates
                .Include(t => t.Author)
                .Where(t => templateIds.Contains(t.Id) && !t.IsDeleted && t.IsPublished && t.IsCurrentVersion)
                .ToListAsync();
        }
    }
}

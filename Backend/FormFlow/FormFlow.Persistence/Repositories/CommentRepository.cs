using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<Comment> CreateAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment> UpdateAsync(Comment comment)
        {
            comment.UpdatedAt = DateTime.UtcNow;
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task DeleteAsync(Guid id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                comment.IsDeleted = true;
                comment.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Comments
                .AnyAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<PagedResult<Comment>> GetCommentsByTemplatePagedAsync(Guid templateId, int page, int pageSize)
        {
            var query = _context.Comments
                .Include(c => c.User)
                .Where(c => c.TemplateId == templateId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt);

            var totalCount = await query.CountAsync();
            var comments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Comment>(comments, totalCount, page, pageSize);
        }

        public async Task<PagedResult<Comment>> GetCommentsByUserPagedAsync(Guid userId, int page, int pageSize)
        {
            var query = _context.Comments
                .Include(c => c.User)
                .Include(c => c.Template)
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt);

            var totalCount = await query.CountAsync();
            var comments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Comment>(comments, totalCount, page, pageSize);
        }

        public async Task<List<Comment>> GetRecentByTemplateAsync(Guid templateId, int count = 10)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Where(c => c.TemplateId == templateId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Comment?> GetWithUserAsync(Guid id)
        {
            return await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<int> GetCountByTemplateAsync(Guid templateId)
        {
            return await _context.Comments
                .CountAsync(c => c.TemplateId == templateId && !c.IsDeleted);
        }

        public async Task<int> GetCountByUserAsync(Guid userId)
        {
            return await _context.Comments
                .CountAsync(c => c.UserId == userId && !c.IsDeleted);
        }

        public async Task<bool> CanUserDeleteAsync(Guid commentId, Guid userId)
        {
            var comment = await _context.Comments
                .Include(c => c.Template)
                .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

            if (comment == null) return false;

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            if (user.Role.HasFlag(UserRole.Admin)) return true;
            if (comment.UserId == userId) return true;

            return comment.Template.AuthorId == userId;
        }

        public async Task<DateTime?> GetLastCommentTimeAsync(Guid templateId)
        {
            var lastComment = await _context.Comments
                .Where(c => c.TemplateId == templateId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync();

            return lastComment?.CreatedAt;
        }

        public async Task<Dictionary<Guid, int>> GetCommentsCountByTemplatesAsync(List<Guid> templateIds)
        {
            return await _context.Comments
                .Where(c => templateIds.Contains(c.TemplateId) && !c.IsDeleted)
                .GroupBy(c => c.TemplateId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<int> GetTotalCommentsCountAsync()
        {
            return await _context.Comments
                .CountAsync(c => !c.IsDeleted);
        }

        public async Task<Dictionary<string, int>> GetCommentsCountByMonthAsync()
        {
            return await _context.Comments
                .Where(c => !c.IsDeleted)
                .GroupBy(c => c.CreatedAt.ToString("yyyy-MM"))
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }
    }
}

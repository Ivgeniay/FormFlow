using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Repositories
{
    public class FormRepository : IFormRepository
    {
        private readonly ApplicationDbContext _context;

        public FormRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Form?> GetByIdAsync(Guid id)
        {
            return await _context.Forms
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);
        }

        public async Task<Form> CreateAsync(Form form)
        {
            _context.Forms.Add(form);
            await _context.SaveChangesAsync();
            return form;
        }

        public async Task<Form> UpdateAsync(Form form)
        {
            form.UpdatedAt = DateTime.UtcNow;
            _context.Forms.Update(form);
            await _context.SaveChangesAsync();
            return form;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Forms
                .AnyAsync(f => f.Id == id && !f.IsDeleted);
        }

        public async Task DeleteAsync(Guid id)
        {
            var form = await _context.Forms.FindAsync(id);
            if (form != null)
            {
                _context.Forms.Remove(form);
                //form.IsDeleted = true;
                //form.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Form?> GetWithTemplateAsync(Guid id)
        {
            return await _context.Forms
                .Include(f => f.Template)
                .ThenInclude(t => t.Author)
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);
        }

        public async Task<Form?> GetWithUserAsync(Guid id)
        {
            return await _context.Forms
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);
        }

        public async Task<Form?> GetWithAllDetailsAsync(Guid id)
        {
            return await _context.Forms
                .Include(f => f.Template)
                .ThenInclude(t => t.Author)
                .Include(f => f.Template.Questions.Where(q => !q.IsDeleted))
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);
        }

        public async Task<PagedResult<Form>> GetFormsByTemplatePagedAsync(Guid templateId, int page, int pageSize)
        {
            var query = _context.Forms
                .Include(f => f.User)
                .Include(f => f.Template)
                .Where(f => f.TemplateId == templateId && !f.IsDeleted)
                .OrderByDescending(f => f.SubmittedAt);

            var totalCount = await query.CountAsync();
            var forms = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Form>(forms, totalCount, page, pageSize);
        }

        public async Task<PagedResult<Form>> GetFormsByUserPagedAsync(Guid userId, int page, int pageSize)
        {
            var query = _context.Forms
                .Include(f => f.Template)
                .ThenInclude(t => t.Author)
                .Include(f => f.User)
                .Where(f => f.UserId == userId && !f.IsDeleted)
                .OrderByDescending(f => f.SubmittedAt);

            var totalCount = await query.CountAsync();
            var forms = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Form>(forms, totalCount, page, pageSize);
        }

        public async Task<bool> HasUserSubmittedAsync(Guid templateId, Guid userId)
        {
            return await _context.Forms
                .AnyAsync(f => f.TemplateId == templateId && f.UserId == userId && !f.IsDeleted);
        }

        public async Task<bool> CanUserEditAsync(Guid formId, Guid userId)
        {
            var form = await _context.Forms
                .Include(f => f.Template)
                .FirstOrDefaultAsync(f => f.Id == formId && !f.IsDeleted);

            if (form == null) return false;

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            if (user.Role.HasFlag(UserRole.Admin)) return true;
            if (form.UserId == userId) return true;

            return form.Template.AuthorId == userId;
        }

        public async Task<bool> CanUserViewAsync(Guid formId, Guid userId)
        {
            var form = await _context.Forms
                .Include(f => f.Template)
                .FirstOrDefaultAsync(f => f.Id == formId && !f.IsDeleted);

            if (form == null) return false;

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            if (user.Role.HasFlag(UserRole.Admin)) return true;
            if (form.UserId == userId) return true;

            return form.Template.AuthorId == userId;
        }

        public async Task<int> GetCountByTemplateAsync(Guid templateId)
        {
            return await _context.Forms
                .CountAsync(f => f.TemplateId == templateId && !f.IsDeleted);
        }

        public async Task<int> GetCountByUserAsync(Guid userId)
        {
            return await _context.Forms
                .CountAsync(f => f.UserId == userId && !f.IsDeleted);
        }

        public async Task<Form?> GetUserFormForTemplateAsync(Guid templateId, Guid userId)
        {
            return await _context.Forms
                .Include(f => f.Template)
                .ThenInclude(t => t.Author)
                .Include(f => f.Template.Questions.Where(q => !q.IsDeleted))
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.TemplateId == templateId && f.UserId == userId && !f.IsDeleted);
        }

        public async Task<List<Form>> GetUserFormsForAllVersionsAsync(Guid baseTemplateId, Guid userId)
        {
            var templateIds = await _context.Templates
                .Where(t => t.BaseTemplateId == baseTemplateId || t.Id == baseTemplateId)
                .Select(t => t.Id)
                .ToListAsync();

            return await _context.Forms
                .Include(f => f.Template)
                .ThenInclude(t => t.Author)
                .Include(f => f.User)
                .Where(f => templateIds.Contains(f.TemplateId) && f.UserId == userId && !f.IsDeleted)
                .OrderByDescending(f => f.SubmittedAt)
                .ToListAsync();
        }

        public async Task<Dictionary<Guid, int>> GetFormsCountByTemplatesAsync(List<Guid> templateIds)
        {
            return await _context.Forms
                .Where(f => templateIds.Contains(f.TemplateId) && !f.IsDeleted)
                .GroupBy(f => f.TemplateId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<int> GetTotalFormsCountAsync()
        {
            return await _context.Forms
                .CountAsync(f => !f.IsDeleted);
        }

        public async Task<Dictionary<string, int>> GetFormsCountByMonthAsync()
        {
            return await _context.Forms
                .Where(f => !f.IsDeleted)
                .GroupBy(f => f.SubmittedAt.ToString("yyyy-MM"))
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<List<Guid>> GetMostActiveUsersAsync(int count)
        {
            return await _context.Forms
                .Where(f => !f.IsDeleted)
                .GroupBy(f => f.UserId)
                .OrderByDescending(g => g.Count())
                .Take(count)
                .Select(g => g.Key)
                .ToListAsync();
        }

        public async Task<PagedResult<Form>> GetAllFormsForAdmin(int page, int pageSize)
        {
            var query = _context.Forms
                .Include(f => f.Template)
                .ThenInclude(t => t.Author)
                .Include(f => f.User)
                .OrderByDescending(f => f.SubmittedAt);

            var totalCount = await query.CountAsync();
            var forms = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Form>(forms, totalCount, page, pageSize);
        }
    }
}

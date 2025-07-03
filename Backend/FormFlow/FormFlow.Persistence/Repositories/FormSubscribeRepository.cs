using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Repositories
{
    public class FormSubscribeRepository : IFormSubscribeRepository
    {
        private readonly ApplicationDbContext _context;

        public FormSubscribeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FormSubscribe> AddAsync(FormSubscribe formSubscribe)
        {
            _context.FormSubscribes.Add(formSubscribe);
            await _context.SaveChangesAsync();
            return formSubscribe;
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid templateId)
        {
            var subscription = await _context.FormSubscribes
                .FirstOrDefaultAsync(fs => fs.UserId == userId && fs.TemplateId == templateId);

            if (subscription == null)
                return false;

            _context.FormSubscribes.Remove(subscription);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistAsync(Guid userId, Guid templateId) =>
            await _context.FormSubscribes
                .AnyAsync(fs => fs.UserId == userId && fs.TemplateId == templateId);
        

        public async Task<bool> HasTemplateSubscribersAsync(Guid templateId) =>
            await _context.FormSubscribes
                .AnyAsync(fs => fs.TemplateId == templateId);
        

        public async Task<bool> HasAnySubscriptionsAsync(Guid userId) =>
            await _context.FormSubscribes
                .AnyAsync(fs => fs.UserId == userId);
        

        public async Task<List<User>> GetByTemplateIdAsync(Guid templateId)
        {
            return await _context.FormSubscribes
                .Where(fs => fs.TemplateId == templateId)
                .Join(_context.Users,
                    fs => fs.UserId,
                    u => u.Id,
                    (fs, u) => u)
                .Where(u => !u.IsDeleted && !u.IsBlocked)
                .Include(u => u.Contacts)
                .ToListAsync();
        }

        public async Task<List<Template>> GetByUserIdAsync(Guid userId)
        {
            return await _context.FormSubscribes
                .Where(fs => fs.UserId == userId)
                .Join(_context.Templates,
                    fs => fs.TemplateId,
                    t => t.Id,
                    (fs, t) => t)
                .Where(t => !t.IsDeleted)
                .Include(t => t.Author)
                .ToListAsync();
        }
    }
}

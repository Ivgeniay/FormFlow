using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Repositories
{
    public class UserContactRepository : IUserContactRepository
    {
        private readonly ApplicationDbContext _context;

        public UserContactRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserContact?> GetByIdAsync(Guid id)
        {
            return await _context.UserContacts
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<UserContact>> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserContacts
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.IsPrimary)
                .ThenBy(c => c.Type)
                .ToListAsync();
        }

        public async Task<UserContact?> GetPrimaryContactAsync(Guid userId)
        {
            return await _context.UserContacts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsPrimary);
        }

        public async Task<UserContact?> GetByValueAsync(string value, ContactType type)
        {
            return await _context.UserContacts
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Value == value && c.Type == type);
        }

        public async Task<UserContact> CreateAsync(UserContact contact)
        {
            if (contact.IsPrimary)
            {
                await ClearPrimaryContactsAsync(contact.UserId, contact.Type);
            }

            _context.UserContacts.Add(contact);
            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task<UserContact> UpdateAsync(UserContact contact)
        {
            if (contact.IsPrimary)
            {
                await ClearPrimaryContactsAsync(contact.UserId, contact.Type);
            }

            contact.UpdatedAt = DateTime.UtcNow;
            _context.UserContacts.Update(contact);
            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task DeleteAsync(Guid id)
        {
            var contact = await _context.UserContacts.FindAsync(id);
            if (contact != null)
            {
                _context.UserContacts.Remove(contact);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteByUserIdAsync(Guid userId)
        {
            var contacts = await _context.UserContacts
                .Where(c => c.UserId == userId)
                .ToListAsync();

            _context.UserContacts.RemoveRange(contacts);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid userId, string value, ContactType type)
        {
            return await _context.UserContacts
                .AnyAsync(c => c.UserId == userId && c.Value == value && c.Type == type);
        }

        private async Task ClearPrimaryContactsAsync(Guid userId, ContactType type)
        {
            var primaryContacts = await _context.UserContacts
                .Where(c => c.UserId == userId && c.Type == type && c.IsPrimary)
                .ToListAsync();

            foreach (var contact in primaryContacts)
            {
                contact.IsPrimary = false;
                contact.UpdatedAt = DateTime.UtcNow;
            }

            if (primaryContacts.Any())
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}

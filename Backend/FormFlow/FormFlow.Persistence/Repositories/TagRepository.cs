using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _context;

        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Tag?> GetByIdAsync(Guid id)
        {
            return await _context.Tags.FindAsync(id);
        }

        public async Task<Tag?> GetByNameAsync(string name)
        {
            var normalizedName = name.Trim().ToLower();
            return await _context.Tags
                .FirstOrDefaultAsync(t => t.Name == normalizedName);
        }

        public async Task<Dictionary<string, Guid>> GetTagIdsByNamesAsync(List<string> names)
        {
            var normalizedNames = names.Select(n => n.Trim().ToLower()).ToList();
            return await _context.Tags
                .Where(t => normalizedNames.Contains(t.Name))
                .ToDictionaryAsync(t => t.Name, t => t.Id);
        }

        public async Task<Tag> CreateAsync(Tag tag)
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag> UpdateAsync(Tag tag)
        {
            tag.UpdatedAt = DateTime.UtcNow;
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Tags.AnyAsync(t => t.Id == id);
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            var normalizedName = name.Trim().ToLower();
            return await _context.Tags.AnyAsync(t => t.Name == normalizedName);
        }

        public async Task DeleteAsync(Guid id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PagedResult<Tag>> GetTagsPagedAsync(int page, int pageSize)
        {
            var query = _context.Tags
                .OrderBy(t => t.Name);

            var totalCount = await query.CountAsync();
            var tags = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Tag>(tags, totalCount, page, pageSize);
        }

        public async Task<List<Tag>> GetMostPopularAsync(int count = 50)
        {
            return await _context.Tags
                .Where(t => t.UsageCount > 0)
                .OrderByDescending(t => t.UsageCount)
                .ThenBy(t => t.Name)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Tag>> SearchByNameAsync(string query, int limit = 10)
        {
            var normalizedQuery = query.Trim().ToLower();
            return await _context.Tags
                .Where(t => t.Name.Contains(normalizedQuery))
                .OrderBy(t => t.Name)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Tag> GetOrCreateByNameAsync(string name)
        {
            var normalizedName = name.Trim().ToLower();
            var existingTag = await GetByNameAsync(normalizedName);

            if (existingTag != null)
                return existingTag;

            var newTag = new Tag
            {
                Name = normalizedName
            };

            return await CreateAsync(newTag);
        }

        public async Task<List<Tag>> GetOrCreateByNamesAsync(List<string> names)
        {
            var tags = new List<Tag>();

            foreach (var name in names)
            {
                var tag = await GetOrCreateByNameAsync(name);
                tags.Add(tag);
            }

            return tags;
        }

        public async Task IncrementUsageCountAsync(Guid tagId)
        {
            var tag = await _context.Tags.FindAsync(tagId);
            if (tag != null)
            {
                tag.UsageCount++;
                await UpdateAsync(tag);
            }
        }
        public async Task IncrementUsageCountAsync(List<Guid> tagIds)
        {
            var tags = await _context.Tags
                .Where(t => tagIds.Contains(t.Id))
                .ToListAsync();

            if (tags != null)
            {
                tags.ForEach(t => t.UsageCount++);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DecrementUsageCountAsync(Guid tagId)
        {
            var tag = await GetByIdAsync(tagId);
            if (tag != null && tag.UsageCount > 0)
            {
                tag.UsageCount--;
                await UpdateAsync(tag);
            }
        }

        public async Task DecrementUsageCountAsync(List<Guid> tagIds)
        {
            var tags = await _context.Tags
                .Where(t => tagIds.Contains(t.Id))
                .ToListAsync();

            if (tags != null)
            {
                tags.ForEach(t => t.UsageCount--);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RecalculateUsageCountAsync(Guid tagId)
        {
            var actualCount = await _context.TemplateTags
                .CountAsync(tt => tt.TagId == tagId);

            var tag = await GetByIdAsync(tagId);
            if (tag != null)
            {
                tag.UsageCount = actualCount;
                await UpdateAsync(tag);
            }
        }

        public async Task CleanupUnusedTagsAsync()
        {
            var unusedTags = await _context.Tags
                .Where(t => t.UsageCount == 0 &&
                           !_context.TemplateTags.Any(tt => tt.TagId == t.Id))
                .ToListAsync();

            if (unusedTags.Any())
            {
                _context.Tags.RemoveRange(unusedTags);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Dictionary<Guid, List<string>>> GetTagsByTemplatesAsync(List<Guid> templateIds)
        {
            var templateTags = await _context.TemplateTags
                .Include(tt => tt.Tag)
                .Where(tt => templateIds.Contains(tt.TemplateId))
                .Select(tt => new { tt.TemplateId, tt.Tag.Name })
                .ToListAsync();

            return templateTags
                .GroupBy(tt => tt.TemplateId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Name).ToList());
        }

        public async Task<List<string>> GetMostUsedTagsByUserAsync(Guid userId)
        {
            return await _context.TemplateTags
                .Include(tt => tt.Tag)
                .Include(tt => tt.Template)
                .Where(tt => tt.Template.AuthorId == userId && !tt.Template.IsDeleted)
                .GroupBy(tt => tt.Tag.Name)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => g.Key)
                .ToListAsync();
        }
    }
}

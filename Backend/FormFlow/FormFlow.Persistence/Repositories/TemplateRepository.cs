using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Repositories
{
    public class TemplateRepository : ITemplateRepository
    {
        private readonly ApplicationDbContext _context;

        public TemplateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Template?> GetByIdUnlimitedAsync(Guid id){
            return await _context.Templates
                .Include(t => t.Author)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Template?> GetByIdAsync(Guid id)
        {
            return await _context.Templates
                .Include(t => t.Author)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }


        public async Task<List<Template>?> GetByIdsAsync(Guid[] ids)
        {
            return await _context
                .Templates
                .Include(t => t.Author)
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();
        }

        public async Task<List<Guid>> GetUserEditableTemplateIdsAsync(Guid[] templateIds, Guid userId)
        {
            return await _context
                .Templates
                .Include(t => t.Author)
                .Where(t => templateIds.Contains(t.Id) && t.Author.Id == userId)
                .Select(t => t.Id)
                .ToListAsync();
        }

        public async Task ArchiveTemplatesAsync(Guid[] templateIds)
        {
            var templates = await _context
                .Templates
                .Where(t => templateIds.Contains(t.Id))
                .ExecuteUpdateAsync(t => 
                    t.SetProperty(x => x.IsArchived, true)
                    .SetProperty(x => x.IsPublished, false)
                );
        }

        public async Task UnarchiveTemplatesAsync(Guid[] templateIds)
        {
            await _context.Templates
                .Where(t => templateIds.Contains(t.Id))
                .ExecuteUpdateAsync(t => t.SetProperty(x => x.IsArchived, false));
        }

        public async Task<Template?> GetCurrentVersionAsync(Guid baseTemplateId)
        {
            return await _context.Templates
                .Include(t => t.Author)
                .FirstOrDefaultAsync(t => t.BaseTemplateId == baseTemplateId && t.IsPublished && !t.IsDeleted);
        }

        public async Task<Template?> GetSpecificVersionAsync(Guid baseTemplateId, int version)
        {
            return await _context.Templates
                .Include(t => t.Author)
                .FirstOrDefaultAsync(t => t.BaseTemplateId == baseTemplateId && t.Version == version && !t.IsDeleted);
        }

        public async Task<List<Template>> GetAllVersionsByBaseAsync(Guid baseTemplateId)
        {
            return await _context.Templates
                .Include(t => t.Author)
                .Where(t => t.BaseTemplateId == baseTemplateId && !t.IsDeleted)
                .OrderBy(t => t.Version)
                .ToListAsync();
        }

        public async Task<Template> CreateAsync(Template template)
        {
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();
            return template;
        }
        public async Task<List<Template>> GetAllVersionsForUserAsync(Guid templateId, Guid forUserId)
        {
            var currentTemplate = await _context.Templates
                .FirstOrDefaultAsync(t => t.Id == templateId && !t.IsDeleted);
            if (currentTemplate == null)
                return new List<Template>();

            Guid baseTemplateId;
            if (currentTemplate.BaseTemplateId.HasValue)
                baseTemplateId = currentTemplate.BaseTemplateId.Value;
            else
                baseTemplateId = templateId;

            return await _context.Templates
                .Include(t => t.Author)
                .Where(t => (t.Id == baseTemplateId || t.BaseTemplateId == baseTemplateId) && t.Author.Id == forUserId)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Template>> GetAllVersionsAsync(Guid templateId)
        {
            var currentTemplate = await _context.Templates
                .FirstOrDefaultAsync(t => t.Id == templateId && !t.IsDeleted);
            if (currentTemplate == null)
                return new List<Template>();

            Guid baseTemplateId;
            if (currentTemplate.BaseTemplateId.HasValue)
                baseTemplateId = currentTemplate.BaseTemplateId.Value;
            else
                baseTemplateId = templateId;
            
            return await _context.Templates
                .Where(t => t.Id == baseTemplateId || t.BaseTemplateId == baseTemplateId)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Template> CreateNewVersionAsync(Template oldVersion, Template newVersion)
        {

            newVersion.Version = await GetLatestVersionNumberAsync(oldVersion.BaseTemplateId ?? oldVersion.Id) + 1;
            newVersion.BaseTemplateId = oldVersion.BaseTemplateId ?? oldVersion.Id;
            newVersion.PreviousVersionId = oldVersion.Id;

            _context.Templates.Add(newVersion);
            await _context.SaveChangesAsync();
            return newVersion;
        }

        public async Task<Template> UpdateAsync(Template template)
        {
            template.UpdatedAt = DateTime.UtcNow;
            _context.Templates.Update(template);
            await _context.SaveChangesAsync();
            return template;
        }

        public async Task<bool> UpdateTemplateImageAsync(Guid templateId, string imageUrl, Guid userId)
        {
            var template = await _context.Templates.FirstOrDefaultAsync(t => t.Id == templateId);
            if (template == null)
            {
                return false;
            }
            template.ImageUrl = imageUrl;
            template.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Templates
                .AnyAsync(t => t.Id == id && !t.IsDeleted);
        }

        public async Task<bool> BaseTemplateExistsAsync(Guid baseTemplateId)
        {
            return await _context.Templates
                .AnyAsync(t => (t.BaseTemplateId == baseTemplateId || t.Id == baseTemplateId) && !t.IsDeleted);
        }

        public async Task UnDeleteAsync(Guid id)
        {
            var template = await _context.Templates.FirstOrDefaultAsync(t => t.Id == id);
            if (template != null)
            {
                template.IsDeleted = false;
                await _context.SaveChangesAsync();
            }
            //await _context.Templates
            //    .Where(t => t.Id == id)
            //    .ExecuteUpdateAsync(t => t.SetProperty(p => p.IsDeleted, false));
        }

        public async Task DeleteAsync(Guid id)
        {
            var template = await _context.Templates.FindAsync(id);
            if (template != null)
            {
                template.IsDeleted = true;
                template.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteTemplatesAsync(Guid[] templateIds)
        {
            await _context.Templates
                .Where(t => templateIds.Contains(t.Id))
                .ExecuteUpdateAsync(t => t.SetProperty(x => x.IsDeleted, true));
        }

        public async Task<Dictionary<Guid, string>> GetTemplateTopicsAsync(List<Guid> templateIds)
        {
            return await _context.Templates
                .Where(t => templateIds.Contains(t.Id) && !t.IsDeleted)
                .Join(_context.Topics,
                    t => t.TopicId,
                    topic => topic.Id,
                    (t, topic) => new { t.Id, topic.Name })
                .ToDictionaryAsync(x => x.Id, x => x.Name);
        }

        public async Task<List<Guid>> GetTemplateAllowedUserIdsAsync(Guid templateId)
        {
            var template = await _context.Templates
                .FirstOrDefaultAsync(t => t.Id == templateId && !t.IsDeleted);

            if (template == null || template.AccessType == TemplateAccess.Public)
                return new List<Guid>();

            return await _context.TemplateAllowedUser
                .Where(tau => tau.TemplateId == templateId)
                .Select(tau => tau.UserId)
                .ToListAsync();
        }

        public async Task<List<Guid>> GetTemplateTagIdsAsync(Guid templateId)
        {
            return await _context.TemplateTags
                .Where(tt => tt.TemplateId == templateId)
                .Select(tt => tt.TagId)
                .ToListAsync();
        }

        public async Task DeleteAllVersionsAsync(Guid baseTemplateId)
        {
            await _context.Templates
                .Where(t => t.BaseTemplateId == baseTemplateId || t.Id == baseTemplateId)
                .ExecuteUpdateAsync(t => t
                    .SetProperty(x => x.IsDeleted, true)
                    .SetProperty(x => x.UpdatedAt, DateTime.UtcNow));
        }

        public async Task<PagedResult<Template>> GetTemplatesByTagNameAsync(string tagName, int page, int pageSize)
        {
            var query = _context.Templates
                .Include(t => t.Author)
                .Where(t => !t.IsDeleted && t.IsPublished &&
                    _context.TemplateTags.Any(tt => tt.TemplateId == t.Id && tt.Tag.Name.ToLower() == tagName.ToLower()))
                .OrderByDescending(t => t.CreatedAt);
            var totalCount = await query.CountAsync();
            var templates = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedResult<Template>(templates, totalCount, page, pageSize);
        }

        public async Task<PagedResult<Template>> GetPopularTemplatesAsync(int page, int pageSize)
        {
            var query = _context.Templates
                .Include(t => t.Author)
                .Where(t => !t.IsDeleted && t.IsPublished)
                .OrderByDescending(t => t.Likes.Count());
            var totalCount = await query.CountAsync();
            var templates = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedResult<Template>(templates, totalCount, page, pageSize);
        }

        public async Task<PagedResult<Template>> GetTemplatesPagedForAdminAsync(int page, int pageSize)
        {
            var query = _context.Templates
                .Include(t => t.Author)
                .Include(t => t.Comments)
                .Include(t => t.Likes)
                .Include(t => t.Forms)
                .OrderByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync();
            var templates = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Template>(templates, totalCount, page, pageSize);
        }

        public async Task<PagedResult<Template>> GetPublicTemplatesPagedAsync(int page, int pageSize)
        {
            var query = _context.Templates
                .Include(t => t.Author)
                .Include(t => t.Questions)
                .Include(t => t.Comments)
                .Include(t => t.Tags)
                    .ThenInclude(tt => tt.Tag)
                .Where(t => !t.IsDeleted && t.AccessType == TemplateAccess.Public && t.IsPublished)
                .OrderByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync();
            var templates = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Template>(templates, totalCount, page, pageSize);
        }

        public async Task<PagedResult<Template>> GetTemplatesByAuthorPagedAsync(Guid authorId, int page, int pageSize)
        {
            var query = _context.Templates
                .Include(t => t.Author)
                .Include(t => t.Likes)
                .Include(t => t.Forms)
                .Where(t => t.AuthorId == authorId && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync();
            var templates = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Template>(templates, totalCount, page, pageSize);
        }

        public async Task<PagedResult<Template>> GetUserAccessibleTemplatesPagedAsync(Guid userId, int page, int pageSize)
        {
            var query = _context.Templates
                .Include(t => t.Author)
                .Include(t => t.AllowedUsers)
                .Where(t => !t.IsDeleted && t.IsPublished &&
                    (t.AccessType == TemplateAccess.Public ||
                     t.AuthorId == userId ||
                     t.AllowedUsers.Any(au => au.UserId == userId)))
                .OrderByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync();
            var templates = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Template>(templates, totalCount, page, pageSize);
        }

        public async Task<List<Template>> GetLatestTemplatesAsync(int count = 10)
        {
            return await _context.Templates
                .Include(t => t.Author)
                .Include(t => t.Likes)
                .Include(t => t.Forms)
                .Where(t => !t.IsDeleted && t.AccessType == TemplateAccess.Public && t.IsPublished)
                .OrderByDescending(t => t.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Template?> GetWithQuestionsAsync(Guid id)
        {
            return await _context.Templates
                .Include(t => t.Author)
                .Include(t => t.Questions.Where(q => !q.IsDeleted))
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        public async Task<Template?> GetWithFormsAsync(Guid id)
        {
            return await _context.Templates
                .Include(t => t.Author)
                .Include(t => t.Forms.Where(f => !f.IsDeleted))
                .ThenInclude(f => f.User)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        public async Task<Template?> GetWithCommentsAsync(Guid id)
        {
            return await _context.Templates
                .Include(t => t.Author)
                .Include(t => t.Comments.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        public async Task<Template?> GetWithAllDetailsAsync(Guid id, bool includeDelete = false)
        {
            return await _context.Templates
                .Include(t => t.Author)
                .Include(t => t.Topic)
                .Include(t => t.Questions.Where(q => !q.IsDeleted))
                .Include(t => t.Tags)
                    .ThenInclude(tt => tt.Tag)
                .Include(t => t.Forms.Where(f => !f.IsDeleted))
                .   ThenInclude(f => f.User)
                .Include(t => t.Comments.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.User)
                .Include(t => t.Likes)
                    .ThenInclude(l => l.User)
                .Include(t => t.AllowedUsers)
                    .ThenInclude(au => au.User)
                .FirstOrDefaultAsync(t => t.Id == id && (includeDelete || !t.IsDeleted));
        }

        public async Task<List<Template>> GetWithAllDetailsAsync(IEnumerable<Guid> ids)
        {
            return await _context.Templates
                .Include(t => t.Author)
                .Include(t => t.Topic)
                .Include(t => t.Questions.Where(q => !q.IsDeleted))
                .Include(t => t.Tags)
                    .ThenInclude(tt => tt.Tag)
                .Include(t => t.Forms.Where(f => !f.IsDeleted))
                .ThenInclude(f => f.User)
                .Include(t => t.Comments.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.User)
                .Include(t => t.Likes)
                    .ThenInclude(l => l.User)
                .Include(t => t.AllowedUsers)
                    .ThenInclude(au => au.User)
                .Where(t => ids.Contains(t.Id) && !t.IsDeleted)
                .ToListAsync() ?? new List<Template>();
        }

        public async Task<List<Template>> GetByTagAsync(Guid tagId)
        {
            return await _context.Templates
                .Include(t => t.Author)
                .Where(t => !t.IsDeleted && t.IsPublished &&
                    _context.TemplateTags.Any(tt => tt.TemplateId == t.Id && tt.TagId == tagId))
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Template>> SearchByTitleOrDescriptionAsync(string query)
        {
            var normalizedQuery = query.ToLower();
            return await _context.Templates
                .Include(t => t.Author)
                .Where(t => !t.IsDeleted && t.IsPublished &&
                    (t.Title.ToLower().Contains(normalizedQuery) ||
                     t.Description.ToLower().Contains(normalizedQuery)))
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasUserAccessAsync(Guid templateId, Guid userId)
        {
            var template = await _context.Templates
                .Include(t => t.AllowedUsers)
                .FirstOrDefaultAsync(t => t.Id == templateId && !t.IsDeleted);

            if (template == null) return false;
            if (template.AuthorId == userId) return true;
            if (template.AccessType == TemplateAccess.Public) return true;

            return template.AllowedUsers.Any(au => au.UserId == userId);
        }

        public async Task<bool> IsAuthorAsync(Guid templateId, Guid userId)
        {
            return await _context.Templates
                .AnyAsync(t => t.Id == templateId && t.AuthorId == userId && !t.IsDeleted);
        }

        public async Task<bool> IsAuthorOfBaseTemplateAsync(Guid baseTemplateId, Guid userId)
        {
            return await _context.Templates
                .AnyAsync(t => (t.BaseTemplateId == baseTemplateId || t.Id == baseTemplateId) &&
                              t.AuthorId == userId && !t.IsDeleted);
        }

        public async Task AddAllowedUserAsync(Guid templateId, Guid userId)
        {
            var exists = await _context.TemplateAllowedUser
                .AnyAsync(tau => tau.TemplateId == templateId && tau.UserId == userId);

            if (!exists)
            {
                var templateAllowedUser = new TemplateAllowedUser
                {
                    TemplateId = templateId,
                    UserId = userId
                };

                _context.TemplateAllowedUser.Add(templateAllowedUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveAllowedUserAsync(Guid templateId, Guid userId)
        {
            var templateAllowedUser = await _context.TemplateAllowedUser
                .FirstOrDefaultAsync(tau => tau.TemplateId == templateId && tau.UserId == userId);

            if (templateAllowedUser != null)
            {
                _context.TemplateAllowedUser.Remove(templateAllowedUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetFormsCountAsync(Guid templateId)
        {
            return await _context.Forms
                .CountAsync(f => f.TemplateId == templateId && !f.IsDeleted);
        }

        public async Task<int> GetFormsCountForVersionAsync(Guid templateId, int version)
        {
            return await _context.Forms
                .CountAsync(f => f.TemplateId == templateId && f.TemplateVersion == version && !f.IsDeleted);
        }

        public async Task<int> GetFormsCountForAllVersionsAsync(Guid baseTemplateId)
        {
            var templateIds = await _context.Templates
                .Where(t => t.BaseTemplateId == baseTemplateId || t.Id == baseTemplateId)
                .Select(t => t.Id)
                .ToListAsync();

            return await _context.Forms
                .CountAsync(f => templateIds.Contains(f.TemplateId) && !f.IsDeleted);
        }

        public async Task<int> GetLikesCountAsync(Guid templateId)
        {
            return await _context.Likes
                .CountAsync(l => l.TemplateId == templateId);
        }

        public async Task<int> GetCommentsCountAsync(Guid templateId)
        {
            return await _context.Comments
                .CountAsync(c => c.TemplateId == templateId && !c.IsDeleted);
        }

        public async Task<int> GetLatestVersionNumberAsync(Guid baseTemplateId)
        {
            var latestVersion = await _context.Templates
                .Where(t => t.BaseTemplateId == baseTemplateId || t.Id == baseTemplateId)
                .MaxAsync(t => (int?)t.Version);

            return latestVersion ?? 0;
        }

        public async Task AddTagToTemplateAsync(Guid templateId, Guid tagId)
        {
            var exists = await _context.TemplateTags
                .AnyAsync(tt => tt.TemplateId == templateId && tt.TagId == tagId);

            if (!exists)
            {
                var templateTag = new TemplateTag
                {
                    TemplateId = templateId,
                    TagId = tagId
                };

                _context.TemplateTags.Add(templateTag);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddTagsToTemplateAsync(Guid templateId, List<Guid> tagIds)
        {
            var existingTagIds = await _context.TemplateTags
                .Where(tt => tt.TemplateId == templateId && tagIds.Contains(tt.TagId))
                .Select(tt => tt.TagId)
                .ToListAsync();

            var newTagIds = tagIds.Except(existingTagIds);

            foreach (var tagId in newTagIds)
            {
                var templateTag = new TemplateTag
                {
                    TemplateId = templateId,
                    TagId = tagId
                };

                _context.TemplateTags.Add(templateTag);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveTagsFromTemplateAsync(Guid templateId, List<Guid> tagIds)
        {
            if (!tagIds.Any()) return;

            var templateTags = await _context.TemplateTags
                .Where(tt => tt.TemplateId == templateId && tagIds.Contains(tt.TagId))
                .ToListAsync();

            if (templateTags.Any())
            {
                _context.TemplateTags.RemoveRange(templateTags);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveTagFromTemplateAsync(Guid templateId, Guid tagId)
        {
            var templateTag = await _context.TemplateTags
                .FirstOrDefaultAsync(tt => tt.TemplateId == templateId && tt.TagId == tagId);

            if (templateTag != null)
            {
                _context.TemplateTags.Remove(templateTag);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveAllTagsFromTemplateAsync(Guid templateId)
        {
            var templateTags = await _context.TemplateTags
                .Where(tt => tt.TemplateId == templateId)
                .ToListAsync();

            if (templateTags.Any())
            {
                _context.TemplateTags.RemoveRange(templateTags);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveAllowedUsersFromTemplateAsync(Guid templateId, List<Guid> userIds)
        {
            if (!userIds.Any()) return;

            var allowedUsers = await _context.TemplateAllowedUser
                .Where(tau => tau.TemplateId == templateId && userIds.Contains(tau.UserId))
                .ToListAsync();

            if (allowedUsers.Any())
            {
                _context.TemplateAllowedUser.RemoveRange(allowedUsers);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddAllowedUsersAsync(Guid templateId, List<Guid> userIds)
        {
            if (!userIds.Any()) return;
            var existingUserIds = await _context.TemplateAllowedUser
                .Where(tau => tau.TemplateId == templateId && userIds.Contains(tau.UserId))
                .Select(tau => tau.UserId)
                .ToListAsync();
            var newUserIds = userIds.Except(existingUserIds);
            if (newUserIds.Any())
            {
                var templateAllowedUsers = newUserIds.Select(userId => new TemplateAllowedUser
                {
                    TemplateId = templateId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                _context.TemplateAllowedUser.AddRange(templateAllowedUsers);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<TemplateTag>> GetTemplateTagsAsync(Guid templateId)
        {
            return await _context.TemplateTags
                .Include(tt => tt.Tag)
                .Where(tt => tt.TemplateId == templateId)
                .ToListAsync();
        }

        public async Task<Dictionary<Guid, int>> GetTemplatesCountByTopicsAsync()
        {
            return await _context.Templates
                .Where(t => !t.IsDeleted && t.IsPublished)
                .GroupBy(t => t.TopicId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<string, int>> GetTemplatesCountByMonthAsync()
        {
            return await _context.Templates
                .Where(t => !t.IsDeleted && t.IsPublished)
                .GroupBy(t => t.CreatedAt.ToString("yyyy-MM"))
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }
    }

}

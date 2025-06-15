using FormFlow.Application.DTOs.Tags;
using FormFlow.Application.DTOs.Templates;
using FormFlow.Application.Interfaces;
using FormFlow.Domain.Exceptions;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly ITemplateRepository _templateRepository;

        public TagService(ITagRepository tagRepository, ITemplateRepository templateRepository)
        {
            _tagRepository = tagRepository;
            _templateRepository = templateRepository;
        }

        public async Task<TagDto> CreateTagAsync(string name)
        {
            var normalizedName = name.Trim().ToLower();

            if (await _tagRepository.NameExistsAsync(normalizedName))
                throw new ArgumentException($"Tag with name '{normalizedName}' already exists");

            var tag = new Tag
            {
                Name = normalizedName
            };

            var createdTag = await _tagRepository.CreateAsync(tag);
            return MapToTagDto(createdTag);
        }

        public async Task<TagDto> GetTagByIdAsync(Guid id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
                throw new TagNotFoundException(id);

            return MapToTagDto(tag);
        }

        public async Task<TagDto> GetTagByNameAsync(string name)
        {
            var normalizedName = name.Trim().ToLower();
            var tag = await _tagRepository.GetByNameAsync(normalizedName);
            if (tag == null)
                throw new TagNotFoundException(normalizedName);

            return MapToTagDto(tag);
        }

        public async Task<bool> TagExistsAsync(string name)
        {
            var normalizedName = name.Trim().ToLower();
            return await _tagRepository.NameExistsAsync(normalizedName);
        }

        public async Task<PagedResult<TagDto>> GetTagsPagedAsync(int page, int pageSize)
        {
            var result = await _tagRepository.GetTagsPagedAsync(page, pageSize);
            var tagDtos = result.Data.Select(MapToTagDto).ToList();

            return new PagedResult<TagDto>(tagDtos, result.Pagination.TotalCount, page, pageSize);
        }

        public async Task<List<TagDto>> GetMostPopularTagsAsync(int count = 50)
        {
            var tags = await _tagRepository.GetMostPopularAsync(count);
            return tags.Select(MapToTagDto).ToList();
        }

        public async Task<List<TagDto>> SearchTagsAsync(string query, int limit = 10)
        {
            var normalizedQuery = query.Trim().ToLower();
            var tags = await _tagRepository.SearchByNameAsync(normalizedQuery, limit);
            return tags.Select(MapToTagDto).ToList();
        }

        public async Task<List<TagDto>> GetOrCreateTagsAsync(List<string> tagNames)
        {
            var tags = await _tagRepository.GetOrCreateByNamesAsync(tagNames);
            return tags.Select(MapToTagDto).ToList();
        }

        public async Task<TagDto> GetOrCreateTagAsync(string name)
        {
            var tag = await _tagRepository.GetOrCreateByNameAsync(name);
            return MapToTagDto(tag);
        }

        public async Task UpdateTagUsageCountAsync(Guid tagId)
        {
            await _tagRepository.RecalculateUsageCountAsync(tagId);
        }

        public async Task IncrementTagUsageAsync(Guid tagId)
        {
            await _tagRepository.IncrementUsageCountAsync(tagId);
        }

        public async Task DecrementTagUsageAsync(Guid tagId)
        {
            await _tagRepository.DecrementUsageCountAsync(tagId);
        }

        public async Task RecalculateTagUsageAsync(Guid tagId)
        {
            await _tagRepository.RecalculateUsageCountAsync(tagId);
        }

        public async Task<List<TemplateListItemDto>> GetTemplatesByTagAsync(Guid tagId, int page, int pageSize)
        {
            var tag = await _tagRepository.GetByIdAsync(tagId);
            if (tag == null)
                throw new TagNotFoundException(tagId);

            var templates = await _templateRepository.GetByTagAsync(tagId);
            var templateDtos = templates.Select(MapToTemplateListItemDto).ToList();

            return templateDtos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public async Task<List<TemplateListItemDto>> GetTemplatesByTagNameAsync(string tagName, int page, int pageSize)
        {
            var normalizedName = tagName.Trim().ToLower();
            var tag = await _tagRepository.GetByNameAsync(normalizedName);
            if (tag == null)
                throw new TagNotFoundException(normalizedName);

            return await GetTemplatesByTagAsync(tag.Id, page, pageSize);
        }

        public async Task CleanupUnusedTagsAsync()
        {
            await _tagRepository.CleanupUnusedTagsAsync();
        }

        public async Task<CloudTagDto> GetTagCloudAsync(int maxTags = 50)
        {
            var tags = await _tagRepository.GetMostPopularAsync(maxTags);

            if (!tags.Any())
            {
                return new CloudTagDto
                {
                    Tags = new List<TagCloudItemDto>(),
                    MaxUsageCount = 0,
                    MinUsageCount = 0,
                    GeneratedAt = DateTime.UtcNow
                };
            }

            var maxUsage = tags.Max(t => t.UsageCount);
            var minUsage = tags.Min(t => t.UsageCount);
            var usageRange = maxUsage - minUsage;

            var cloudItems = tags.Select(tag => new TagCloudItemDto
            {
                Id = tag.Id,
                Name = tag.Name,
                UsageCount = tag.UsageCount,
                Weight = CalculateTagWeight(tag.UsageCount, minUsage, usageRange)
            }).ToList();

            return new CloudTagDto
            {
                Tags = cloudItems,
                MaxUsageCount = maxUsage,
                MinUsageCount = minUsage,
                GeneratedAt = DateTime.UtcNow
            };
        }

        private static int CalculateTagWeight(int usageCount, int minUsage, int usageRange)
        {
            if (usageRange == 0) return 3;

            var normalizedUsage = (double)(usageCount - minUsage) / usageRange;
            return (int)Math.Ceiling(normalizedUsage * 5) + 1;
        }

        private static TagDto MapToTagDto(Tag tag)
        {
            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                UsageCount = tag.UsageCount,
                CreatedAt = tag.CreatedAt
            };
        }

        private static TemplateListItemDto MapToTemplateListItemDto(Template template)
        {
            return new TemplateListItemDto
            {
                Id = template.Id,
                Title = template.Title,
                Description = template.Description,
                TopicId = template.TopicId,
                ImageUrl = template.ImageUrl,
                AuthorName = template.Author?.UserName ?? "Unknown Author",
                CreatedAt = template.CreatedAt,
                Tags = template.Tags?.Select(tt => tt.Tag.Name).ToList() ?? new List<string>(),
                FormsCount = template.FormsCount,
                LikesCount = template.LikesCount
            };
        }
    }
}

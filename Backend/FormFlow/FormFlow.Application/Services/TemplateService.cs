using FormFlow.Domain.Models.General.QuestionDetailsModels;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.SearchService;
using FormFlow.Domain.Interfaces.Services;
using FormFlow.Application.DTOs.Templates;
using FormFlow.Application.DTOs.Users;
using FormFlow.Application.Interfaces;
using FormFlow.Domain.Exceptions;
using FormFlow.Domain.Models.General;
using System.Text.Json;

namespace FormFlow.Application.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateRepository _templateRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ISearchService _searchService;
        private readonly ITopicRepository _topicRepository;
        private readonly IQuestionRepository _questionRepository;

        public TemplateService(
            ITemplateRepository templateRepository,
            IUserRepository userRepository,
            ITagRepository tagRepository,
            ISearchService searchService,
            ITopicRepository topicRepository, 
            IQuestionRepository questionRepository)
        {
            _templateRepository = templateRepository;
            _userRepository = userRepository;
            _tagRepository = tagRepository;
            _searchService = searchService;
            _topicRepository = topicRepository;
            _questionRepository = questionRepository;
        }

        public async Task<TemplateDto> CreateTemplateAsync(CreateTemplateRequest request, Guid authorId)
        {
            var author = await _userRepository.GetByIdAsync(authorId);
            if (author == null)
                throw new UserNotFoundException(authorId);

            if (author.IsBlocked)
                throw new UserBlockedException(authorId);

            var template = new Template
            {
                Title = request.Title,
                TopicId = request.TopicId,
                Description = request.Description,
                AccessType = request.AccessType,
                AuthorId = authorId,
                IsPublished = false,
                Version = 1
            };

            var createdTemplate = await _templateRepository.CreateAsync(template);
            if (request.Questions.Any())
            {
                var questions = request.Questions.Select(q => new Question
                {
                    TemplateId = createdTemplate.Id,
                    Order = q.Order,
                    ShowInResults = q.ShowInResults,
                    IsRequired = q.IsRequired,
                    Data = q.Data
                }).ToList();

                await _questionRepository.CreateRangeForTemplateAsync(questions);
            }

            if (request.Tags.Any())
            {
                var tags = await _tagRepository.GetOrCreateByNamesAsync(request.Tags);
                await _templateRepository.AddTagsToTemplateAsync(createdTemplate.Id, tags.Select(t => t.Id).ToList());
            }

            if (request.AllowedUserIds.Any())
                await _templateRepository.AddAllowedUsersAsync(createdTemplate.Id, request.AllowedUserIds);
            
            await IndexTemplateAsync(createdTemplate.Id);

            return await MapToTemplateDtoAsync(createdTemplate, authorId);
        }

        public async Task<TemplateDto> CreateNewVersionAsync(CreateNewVersionRequest request, Guid userId)
        {
            var baseTemplate = await _templateRepository.GetByIdAsync(request.BaseTemplateId);
            if (baseTemplate == null)
                throw new TemplateNotFoundException(request.BaseTemplateId);

            if (!await _templateRepository.IsAuthorOfBaseTemplateAsync(baseTemplate.BaseTemplateId ?? baseTemplate.Id, userId))
                throw new UnauthorizedAccessException("Only template author can create new versions");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.IsBlocked)
                throw new UserBlockedException(userId);

            var newVersion = new Template
            {
                Title = request.Title,
                Description = request.Description,
                AccessType = request.AccessType,
                TopicId = request.TopicId,
                AuthorId = userId,
                IsPublished = false
            };

            foreach (var questionDto in request.Questions)
            {
                var question = new Question
                {
                    TemplateId = newVersion.Id,
                    Order = questionDto.Order,
                    ShowInResults = questionDto.ShowInResults,
                    IsRequired = questionDto.IsRequired,
                    Data = questionDto.Data
                };
                newVersion.Questions.Add(question);
            }

            var createdVersion = await _templateRepository.CreateNewVersionAsync(baseTemplate, newVersion);

            if (request.Tags.Any())
            {
                var tags = await _tagRepository.GetOrCreateByNamesAsync(request.Tags);
                await _templateRepository.AddTagsToTemplateAsync(createdVersion.Id, tags.Select(t => t.Id).ToList());
            }

            if (request.AllowedUserIds.Any())
                await _templateRepository.AddAllowedUsersAsync(createdVersion.Id, request.AllowedUserIds);


            await IndexTemplateAsync(createdVersion.Id);

            return await MapToTemplateDtoAsync(createdVersion, userId);
        }


        public async Task<TemplateDto> UpdateTemplateAsync(UpdateTemplateRequest request, Guid userId)
        {
            var template = await _templateRepository.GetWithAllDetailsAsync(request.Id);
            if (template == null)
                throw new TemplateNotFoundException(request.Id);

            if (!await CanUserEditTemplateAsync(request.Id, userId))
                throw new UnauthorizedAccessException("User cannot edit this template");

            template.Title = request.Title;
            template.Description = request.Description;
            template.AccessType = request.AccessType;
            template.TopicId = request.TopicId;

            template.Questions.Clear();
            foreach (var questionDto in request.Questions)
            {
                var question = new Question
                {
                    Id = questionDto.Id,
                    TemplateId = template.Id,
                    Order = questionDto.Order,
                    ShowInResults = questionDto.ShowInResults,
                    IsRequired = questionDto.IsRequired,
                    Data = questionDto.Data
                };
                template.Questions.Add(question);
            }

            var existingTagIds = await _templateRepository.GetTemplateTagIdsAsync(template.Id);
            var tags = await _tagRepository.GetOrCreateByNamesAsync(request.Tags);
            var newTagIds = tags.Select(t => t.Id).ToList();

            var tagsToRemove = existingTagIds.Except(newTagIds).ToList();
            var tagsToAdd = newTagIds.Except(existingTagIds).ToList();

            if (tagsToRemove.Any())
                await _templateRepository.RemoveTagsFromTemplateAsync(template.Id, tagsToRemove);
            if (tagsToAdd.Any())
                await _templateRepository.AddTagsToTemplateAsync(template.Id, tagsToAdd);

            var existingAllowedUserIds = await _templateRepository.GetTemplateAllowedUserIdsAsync(template.Id);
            var usersToRemove = existingAllowedUserIds.Except(request.AllowedUserIds).ToList();
            var usersToAdd = request.AllowedUserIds.Except(existingAllowedUserIds).ToList();

            if (usersToRemove.Any())
                await _templateRepository.RemoveAllowedUsersFromTemplateAsync(template.Id, usersToRemove);
            if (usersToAdd.Any())
                await _templateRepository.AddAllowedUsersAsync(template.Id, usersToAdd);

            var updatedTemplate = await _templateRepository.UpdateAsync(template);
            await IndexTemplateAsync(updatedTemplate.Id);
            return await MapToTemplateDtoAsync(updatedTemplate, userId);
        }

        public async Task<TemplateDto> UpdateTemplateAllowedUsersAsync(Guid templateId, UpdateTemplateAllowedUsersRequest request, Guid userId)
        {
            if (!await CanUserEditTemplateAsync(templateId, userId))
                throw new UnauthorizedAccessException("User cannot edit this template");

            var existingUserIds = await _templateRepository.GetTemplateAllowedUserIdsAsync(templateId);
            var newUserIds = request.AllowedUserIds;

            var usersToRemove = existingUserIds.Except(newUserIds).ToList();
            var usersToAdd = newUserIds.Except(existingUserIds).ToList();

            if (usersToRemove.Any())
            {
                await _templateRepository.RemoveAllowedUsersFromTemplateAsync(templateId, usersToRemove);
            }

            if (usersToAdd.Any())
                await _templateRepository.AddAllowedUsersAsync(templateId, usersToAdd);

            await IndexTemplateAsync(templateId);

            var template = await _templateRepository.GetWithAllDetailsAsync(templateId);
            return await MapToTemplateDtoAsync(template!, userId);
        }

        public async Task<TemplateDto> PublishTemplateAsync(Guid templateId, Guid userId)
        {
            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            if (!await CanUserEditTemplateAsync(templateId, userId))
                throw new UnauthorizedAccessException("User cannot publish this template");

            var allVersions = await _templateRepository.GetAllVersionsAsync(templateId);
            foreach (var version in allVersions)
            {
                if (version.Id != templateId && version.IsPublished)
                {
                    version.IsPublished = false;
                    await _templateRepository.UpdateAsync(version);
                }
            }

            template.IsPublished = true;
            template.IsArchived = false;
            var updatedTemplate = await _templateRepository.UpdateAsync(template);

            await IndexTemplateAsync(updatedTemplate.Id);

            return await MapToTemplateDtoAsync(updatedTemplate, userId);
        }

        public async Task<TemplateDto> ArchiveTemplateAsync(Guid templateId, Guid userId)
        {
            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            if (!await CanUserEditTemplateAsync(templateId, userId))
                throw new UnauthorizedAccessException("User cannot archive this template");

            template.IsPublished = false;
            template.IsArchived = true;
            var updatedTemplate = await _templateRepository.UpdateAsync(template);

            await IndexTemplateAsync(updatedTemplate.Id);

            return await MapToTemplateDtoAsync(updatedTemplate, userId);
        }

        public async Task<bool> ArchiveTemplatesAsync(Guid[] templateIds, Guid userId)
        {
            var templates = await _templateRepository.GetByIdsAsync(templateIds);

            var foundId = templates.Select(e => e.Id).ToHashSet();
            var notFound = templateIds.Except(foundId);
            if (notFound.Any())
            {
                throw new TemplateNotFoundException(notFound.First());
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) 
                throw new UserNotFoundException(userId);

            if (!user.Role.HasFlag(UserRole.Admin))
            {
                var editableTemplateIds = await _templateRepository.GetUserEditableTemplateIdsAsync(templateIds, userId);
                var nonEditableIds = templateIds.Except(editableTemplateIds).ToList();
                if (nonEditableIds.Any())
                    throw new UnauthorizedAccessException($"Cannot edit templates: {string.Join(", ", nonEditableIds)}");
            }

            await _templateRepository.ArchiveTemplatesAsync(templateIds);

            foreach (var templateId in templateIds)
            {
                await IndexTemplateAsync(templateId);
            }
            return true;
        }

        public async Task<bool> UnarchiveTemplatesAsync(Guid[] templateIds, Guid userId)
        {
            var templates = await _templateRepository.GetByIdsAsync(templateIds);

            var foundIds = templates.Select(t => t.Id).ToHashSet();
            var notFound = templateIds.Except(foundIds);
            if (notFound.Any())
                throw new TemplateNotFoundException(notFound.First());

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            if (!user.Role.HasFlag(UserRole.Admin))
            {
                var editableTemplateIds = await _templateRepository.GetUserEditableTemplateIdsAsync(templateIds, userId);
                var nonEditableIds = templateIds.Except(editableTemplateIds).ToList();
                if (nonEditableIds.Any())
                    throw new UnauthorizedAccessException($"Cannot edit templates: {string.Join(", ", nonEditableIds)}");
            }

            await _templateRepository.UnarchiveTemplatesAsync(templateIds);

            foreach (var templateId in templateIds)
            {
                await IndexTemplateAsync(templateId);
            }

            return true;
        }

        public async Task DeleteTemplateAsync(Guid templateId, Guid userId)
        {
            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            if (!await CanUserEditTemplateAsync(templateId, userId))
                throw new UnauthorizedAccessException("User cannot delete this template");

            await _templateRepository.DeleteAsync(templateId);

            await IndexTemplateAsync(templateId);
        }

        public async Task<bool> DeleteTemplatesAsync(Guid[] templateIds, Guid userId)
        {
            var templates = await _templateRepository.GetByIdsAsync(templateIds);

            var foundIds = templates.Select(t => t.Id).ToHashSet();
            var notFound = templateIds.Except(foundIds);
            if (notFound.Any())
                throw new TemplateNotFoundException(notFound.First());

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            if (!user.Role.HasFlag(UserRole.Admin))
            {
                var editableTemplateIds = await _templateRepository.GetUserEditableTemplateIdsAsync(templateIds, userId);
                var nonEditableIds = templateIds.Except(editableTemplateIds).ToList();
                if (nonEditableIds.Any())
                    throw new UnauthorizedAccessException($"Cannot edit templates: {string.Join(", ", nonEditableIds)}");
            }

            await _templateRepository.DeleteTemplatesAsync(templateIds);

            foreach (var templateId in templateIds)
            {
                await IndexTemplateAsync(templateId);
            }

            return true;
        }

        public async Task DeleteAllVersionsAsync(Guid baseTemplateId, Guid userId)
        {
            if (!await _templateRepository.BaseTemplateExistsAsync(baseTemplateId))
                throw new TemplateNotFoundException(baseTemplateId);

            if (!await _templateRepository.IsAuthorOfBaseTemplateAsync(baseTemplateId, userId))
                throw new UnauthorizedAccessException("Only template author can delete all versions");

            var versions = await _templateRepository.GetAllVersionsByBaseAsync(baseTemplateId);

            await _templateRepository.DeleteAllVersionsAsync(baseTemplateId);

            foreach (var version in versions)
            {
                await IndexTemplateAsync(version.Id);
            }
        }

        public async Task<TemplateDto> GetTemplateByIdAsync(Guid id, Guid? userId = null)
        {
            var template = await _templateRepository.GetWithAllDetailsAsync(id);
            if (template == null)
                throw new TemplateNotFoundException(id);;

            return await MapToTemplateDtoAsync(template, userId);
        }

        public async Task<TemplateDto> GetCurrentVersionAsync(Guid baseTemplateId, Guid? userId = null)
        {
            var template = await _templateRepository.GetCurrentVersionAsync(baseTemplateId);
            if (template == null)
                throw new TemplateNotFoundException(baseTemplateId);

            if (userId.HasValue && !await HasUserAccessToTemplateAsync(template.Id, userId.Value))
                throw new TemplateAccessDeniedException(template.Id, userId.Value);

            return await MapToTemplateDtoAsync(template, userId);
        }

        public async Task<TemplateDto> GetSpecificVersionAsync(Guid baseTemplateId, int version, Guid? userId = null)
        {
            var template = await _templateRepository.GetSpecificVersionAsync(baseTemplateId, version);
            if (template == null)
                throw new TemplateNotFoundException(baseTemplateId);

            if (userId.HasValue && !await HasUserAccessToTemplateAsync(template.Id, userId.Value))
                throw new TemplateAccessDeniedException(template.Id, userId.Value);

            return await MapToTemplateDtoAsync(template, userId);
        }

        public async Task<List<TemplateDto>> GetAllVersionsAsync(Guid templateId, Guid userId)
        {
            var templates = await _templateRepository.GetAllVersionsAsync(templateId);
            var templateDtos = new List<TemplateDto>();

            foreach (var template in templates)
            {
                templateDtos.Add(await MapToTemplateDtoAsync(template, userId));
            }

            return templateDtos;
        }

        public Task<bool> UpdateTemplateImageAsync(Guid templateId, string imageUrl, Guid userId) =>
            _templateRepository.UpdateTemplateImageAsync(templateId, imageUrl, userId);

        public async Task<bool> TemplateExistsAsync(Guid id)
        {
            return await _templateRepository.ExistsAsync(id);
        }

        public async Task<bool> BaseTemplateExistsAsync(Guid baseTemplateId)
        {
            return await _templateRepository.BaseTemplateExistsAsync(baseTemplateId);
        }

        public async Task<PagedResult<TemplateDto>> GetPublicTemplatesPagedAsync(int page, int pageSize)
        {
            var result = await _templateRepository.GetPublicTemplatesPagedAsync(page, pageSize);
            var templateDtos = new List<TemplateDto>();

            foreach (var template in result.Data)
            {
                templateDtos.Add(await MapToTemplateDtoAsync(template, null));
            }

            return new PagedResult<TemplateDto>(templateDtos, result.Pagination.TotalCount, page, pageSize);
        }

        public async Task<PagedResult<TemplateDto>> GetTemplatesByAuthorPagedAsync(Guid authorId, int page, int pageSize)
        {
            var result = await _templateRepository.GetTemplatesByAuthorPagedAsync(authorId, page, pageSize);
            var templateDtos = new List<TemplateDto>();

            foreach (var template in result.Data)
            {
                templateDtos.Add(await MapToTemplateDtoAsync(template, authorId));
            }

            return new PagedResult<TemplateDto>(templateDtos, result.Pagination.TotalCount, page, pageSize);
        }

        public async Task<PagedResult<TemplateDto>> GetUserAccessibleTemplatesPagedAsync(Guid userId, int page, int pageSize)
        {
            var result = await _templateRepository.GetUserAccessibleTemplatesPagedAsync(userId, page, pageSize);
            var templateDtos = new List<TemplateDto>();

            foreach (var template in result.Data)
            {
                templateDtos.Add(await MapToTemplateDtoAsync(template, userId));
            }

            return new PagedResult<TemplateDto>(templateDtos, result.Pagination.TotalCount, page, pageSize);
        }
        
        public async Task<PagedResult<TemplateDto>> GetTemplatesByTagNameAsync(string tagName, int page, int pageSize)
        {
            var result = await _templateRepository.GetTemplatesByTagNameAsync(tagName, page, pageSize);
            var templateDtos = new List<TemplateDto>();
            foreach (var template in result.Data)
            {
                templateDtos.Add(await MapToTemplateDtoAsync(template, null));
            }

            return new PagedResult<TemplateDto>(templateDtos, result.Pagination.TotalCount, page, pageSize);
        }

        public async Task<PagedResult<TemplateDto>> GetPopularTemplatesAsync(int page, int pageSize)
        {
            var result = await _templateRepository.GetPopularTemplatesAsync(page, pageSize);
            var templateDtos = new List<TemplateDto>();
            foreach (var template in result.Data)
            {
                templateDtos.Add(await MapToTemplateDtoAsync(template, null));
            }

            return new PagedResult<TemplateDto>(templateDtos, result.Pagination.TotalCount, page, pageSize);
        }

        public async Task<List<TemplateDto>> GetLatestTemplatesAsync(int count = 10)
        {
            var templates = await _templateRepository.GetLatestTemplatesAsync(count);
            var templateDtos = new List<TemplateDto>();

            foreach (var template in templates)
            {
                templateDtos.Add(await MapToTemplateDtoAsync(template, null));
            }

            return templateDtos;
        }

        public async Task AddTagToTemplateAsync(Guid templateId, string tagName, Guid userId)
        {
            if (!await CanUserEditTemplateAsync(templateId, userId))
                throw new UnauthorizedAccessException("User cannot edit this template");

            var tag = await _tagRepository.GetOrCreateByNameAsync(tagName);
            await _templateRepository.AddTagToTemplateAsync(templateId, tag.Id);
        }

        public async Task RemoveTagFromTemplateAsync(Guid templateId, Guid tagId, Guid userId)
        {
            if (!await CanUserEditTemplateAsync(templateId, userId))
                throw new UnauthorizedAccessException("User cannot edit this template");

            await _templateRepository.RemoveTagFromTemplateAsync(templateId, tagId);
        }

        public async Task<TemplateDto> UpdateTemplateTagsAsync(Guid templateId, UpdateTemplateTagsRequest request, Guid userId)
        {
            if (!await CanUserEditTemplateAsync(templateId, userId))
                throw new UnauthorizedAccessException("User cannot edit this template");

            var existingTagIds = await _templateRepository.GetTemplateTagIdsAsync(templateId);

            var existingTagsByName = await _tagRepository.GetTagIdsByNamesAsync(request.Tags);
            var missingTagNames = request.Tags.Except(existingTagsByName.Keys).ToList();

            if (missingTagNames.Any())
            {
                var newTags = await _tagRepository.GetOrCreateByNamesAsync(missingTagNames);
                foreach (var tag in newTags)
                {
                    existingTagsByName[tag.Name] = tag.Id;
                }
            }

            var newTagIds = existingTagsByName.Values.ToList();
            var tagsToRemove = existingTagIds.Except(newTagIds).ToList();
            var tagsToAdd = newTagIds.Except(existingTagIds).ToList();

            if (tagsToRemove.Any())
            {
                await _templateRepository.RemoveTagsFromTemplateAsync(templateId, tagsToRemove);
            }

            if (tagsToAdd.Any())
            {
                await _templateRepository.AddTagsToTemplateAsync(templateId, tagsToAdd);
            }

            await IndexTemplateAsync(templateId);

            var template = await _templateRepository.GetWithAllDetailsAsync(templateId);
            return await MapToTemplateDtoAsync(template!, userId);
        }

        public async Task AddAllowedUserToTemplateAsync(Guid templateId, Guid allowedUserId, Guid ownerId)
        {
            if (!await CanUserEditTemplateAsync(templateId, ownerId))
                throw new UnauthorizedAccessException("User cannot edit this template");

            var user = await _userRepository.GetByIdAsync(allowedUserId);
            if (user == null)
                throw new UserNotFoundException(allowedUserId);

            await _templateRepository.AddAllowedUserAsync(templateId, allowedUserId);
        }

        public async Task RemoveAllowedUserFromTemplateAsync(Guid templateId, Guid allowedUserId, Guid ownerId)
        {
            if (!await CanUserEditTemplateAsync(templateId, ownerId))
                throw new UnauthorizedAccessException("User cannot edit this template");

            await _templateRepository.RemoveAllowedUserAsync(templateId, allowedUserId);
        }

        public async Task<List<UserSearchDto>> GetTemplateAllowedUsersAsync(Guid templateId, Guid userId)
        {
            if (!await CanUserEditTemplateAsync(templateId, userId))
                throw new UnauthorizedAccessException("User cannot view template allowed users");

            var template = await _templateRepository.GetWithAllDetailsAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            return template.AllowedUsers.Select(au => new UserSearchDto
            {
                Id = au.User.Id,
                UserName = au.User.UserName,
                PrimaryEmail = au.User.Contacts?.FirstOrDefault(c => c.IsPrimary && c.Type == ContactType.Email)?.Value ?? string.Empty
            }).ToList();
        }

        public async Task<bool> HasUserAccessToTemplateAsync(Guid templateId, Guid userId)
        {
            return await _templateRepository.HasUserAccessAsync(templateId, userId);
        }

        public async Task<bool> IsUserTemplateAuthorAsync(Guid templateId, Guid userId)
        {
            return await _templateRepository.IsAuthorAsync(templateId, userId);
        }

        public async Task<bool> IsUserBaseTemplateAuthorAsync(Guid baseTemplateId, Guid userId)
        {
            return await _templateRepository.IsAuthorOfBaseTemplateAsync(baseTemplateId, userId);
        }

        public async Task<bool> CanUserEditTemplateAsync(Guid templateId, Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            if (user.Role.HasFlag(UserRole.Admin)) return true;

            return await _templateRepository.IsAuthorAsync(templateId, userId);
        }

        public async Task<bool> CanUserCreateNewVersionAsync(Guid baseTemplateId, Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            if (user.Role.HasFlag(UserRole.Admin)) return true;

            return await _templateRepository.IsAuthorOfBaseTemplateAsync(baseTemplateId, userId);
        }

        public async Task RemoveTemplateImageAsync(Guid templateId, Guid userId)
        {
            if (!await CanUserEditTemplateAsync(templateId, userId))
                throw new UnauthorizedAccessException("User cannot edit this template");

            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            template.ImageUrl = null;
            await _templateRepository.UpdateAsync(template);
        }

        public async Task<TemplateVersionInfoDto> GetVersionInfoAsync(Guid baseTemplateId, Guid userId)
        {
            if (!await _templateRepository.IsAuthorOfBaseTemplateAsync(baseTemplateId, userId))
                throw new UnauthorizedAccessException("Only template author can view version info");

            var versions = await _templateRepository.GetAllVersionsByBaseAsync(baseTemplateId);
            if (!versions.Any())
                throw new TemplateNotFoundException(baseTemplateId);

            var firstTemplate = versions.First();

            return new TemplateVersionInfoDto
            {
                BaseTemplateId = baseTemplateId,
                TemplateName = firstTemplate.Title,
                AuthorName = firstTemplate.Author?.UserName ?? string.Empty,
                TotalVersions = versions.Count,
                FirstCreated = versions.Min(v => v.CreatedAt),
                LastUpdated = versions.Max(v => v.UpdatedAt),
                Versions = versions.Select(v => new TemplateVersionSummaryDto
                {
                    Id = v.Id,
                    Version = v.Version,
                    Title = v.Title,
                    IsPublished = v.IsPublished,
                    IsArchived = v.IsArchived,
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt,
                    FormsCount = v.FormsCount,
                    QuestionsCount = v.Questions?.Count ?? 0
                }).ToList()
            };
        }

        public async Task<TemplateDto> MapToTemplateDtoAsync(Template template, Guid? userId)
        {
            var templateTags = await _templateRepository.GetTemplateTagsAsync(template.Id);

            var topicName = template.Topic?.Name ??
                (await _templateRepository.GetTemplateTopicsAsync(new List<Guid> { template.Id }))
                .GetValueOrDefault(template.Id, "Other");

            var hasAccess = userId.HasValue ? await HasUserAccessToTemplateAsync(template.Id, userId.Value) : template.AccessType == TemplateAccess.Public;
            var canEdit = userId.HasValue ? await CanUserEditTemplateAsync(template.Id, userId.Value) : false;

            var dto = new TemplateDto
            {
                Id = template.Id,
                Title = template.Title,
                Description = template.Description,
                TopicId = template.TopicId,
                Topic = topicName,
                ImageUrl = template.ImageUrl,
                AccessType = template.AccessType,
                AuthorId = template.AuthorId,
                AuthorName = template.Author?.UserName ?? string.Empty,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt,
                Version = template.Version,
                IsPublished = template.IsPublished,
                IsArchived = template.IsArchived,
                BaseTemplateId = template.BaseTemplateId,
                PreviousVersionId = template.PreviousVersionId,
                Questions = template.Questions?.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Order = q.Order,
                    ShowInResults = q.ShowInResults,
                    IsRequired = q.IsRequired,
                    Data = q.Data,
                    CreatedAt = q.CreatedAt
                }).ToList() ?? new List<QuestionDto>(),
                Tags = templateTags.Select(tt => new DTOs.Tags.TagDto
                {
                    Id = tt.Tag.Id,
                    Name = tt.Tag.Name,
                    UsageCount = tt.Tag.UsageCount,
                    CreatedAt = tt.Tag.CreatedAt
                }).ToList(),
                AllowedUsers = template.AllowedUsers?.Select(au => new UserSearchDto
                {
                    Id = au.User.Id,
                    UserName = au.User.UserName,
                    PrimaryEmail = au.User.Contacts?.FirstOrDefault(c => c.IsPrimary && c.Type == ContactType.Email)?.Value ?? string.Empty
                }).ToList() ?? new List<UserSearchDto>(),
                FormsCount = template.FormsCount,
                LikesCount = template.LikesCount,
                CommentsCount = template.CommentsCount,
                IsUserLiked = false,
                HasUserAccess = hasAccess,
                CanUserEdit = canEdit
            };

            return dto;
        }

        private async Task IndexTemplateAsync(Guid templateId)
        {
            try
            {
                var template = await _templateRepository.GetWithAllDetailsAsync(templateId);
                if (template == null)
                {
                    await _searchService.RemoveTemplateFromIndexAsync(templateId);
                    return;
                }

                var searchDocument = await BuildTemplateSearchDocumentAsync(template);
                await _searchService.UpdateTemplateIndexAsync(searchDocument);
            }
            catch
            {
            }
        }

        private async Task<TemplateSearchDocument> BuildTemplateSearchDocumentAsync(Template template)
        {
            var templateTags = await _templateRepository.GetTemplateTagsAsync(template.Id);

            var questionsText = template.Questions?
                .Where(q => !q.IsDeleted)
                .Select(q =>
                {
                    try
                    {
                        var questionDetails = JsonSerializer.Deserialize<QuestionDetails>(q.Data);
                        return $"{questionDetails?.Title} {questionDetails?.Description}";
                    }
                    catch
                    {
                        return "";
                    }
                })
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .Aggregate("", (current, text) => current + " " + text)
                .Trim() ?? "";

            var commentsText = template.Comments?
                .Where(c => !c.IsDeleted)
                .Select(c => c.Content)
                .Where(content => !string.IsNullOrWhiteSpace(content))
                .Aggregate("", (current, content) => current + " " + content)
                .Trim() ?? "";

            var topicNames = await _templateRepository.GetTemplateTopicsAsync(new List<Guid> { template.Id });
            var topicName = topicNames.GetValueOrDefault(template.Id, "Other");

            return new TemplateSearchDocument
            {
                Id = template.Id,
                Title = template.Title,
                Topic = topicName,
                Description = template.Description,
                QuestionsText = questionsText,
                CommentsText = commentsText,
                AuthorName = template.Author?.UserName ?? "",
                Tags = templateTags.Select(tt => tt.Tag.Name).ToList(),
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt,
                FormsCount = template.FormsCount,
                LikesCount = template.LikesCount,
                CommentsCount = template.CommentsCount,
                IsArchived = template.IsArchived,
                IsPublished = template.IsPublished,
                IsDeleted = template.IsDeleted
            };
        }
    }

}

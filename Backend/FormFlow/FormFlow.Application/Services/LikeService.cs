using FormFlow.Application.DTOs.Likes;
using FormFlow.Application.DTOs.Templates;
using FormFlow.Application.Interfaces;
using FormFlow.Domain.Exceptions;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IUserRepository _userRepository;

        public LikeService(
            ILikeRepository likeRepository,
            ITemplateRepository templateRepository,
            IUserRepository userRepository)
        {
            _likeRepository = likeRepository;
            _templateRepository = templateRepository;
            _userRepository = userRepository;
        }

        public async Task<LikeResultDto> ToggleLikeAsync(Guid userId, Guid templateId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            if (user.IsBlocked)
                throw new UserBlockedException(userId);

            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            if (!template.IsPublished || template.AccessType != TemplateAccess.Public)
                throw new UnauthorizedAccessException("Cannot like non-public or unpublished templates");

            var existingLike = await _likeRepository.GetByTemplateAndUserAsync(templateId, userId);
            bool isLiked;
            string action;

            if (existingLike != null)
            {
                await _likeRepository.DeleteAsync(existingLike.Id);
                isLiked = false;
                action = "removed";
            }
            else
            {
                var like = new Like
                {
                    TemplateId = templateId,
                    UserId = userId
                };
                await _likeRepository.CreateAsync(like);
                isLiked = true;
                action = "added";
            }

            var totalLikes = await _likeRepository.GetCountByTemplateAsync(templateId);

            return new LikeResultDto
            {
                IsLiked = isLiked,
                TotalLikes = totalLikes,
                Action = action,
                LastLikeUserId = isLiked ? userId : null,
                LastLikeUserName = isLiked ? user.UserName : null
            };
        }

        public async Task<bool> AddLikeAsync(Guid userId, Guid templateId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            if (user.IsBlocked)
                throw new UserBlockedException(userId);

            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            if (!template.IsPublished || template.AccessType != TemplateAccess.Public)
                throw new UnauthorizedAccessException("Cannot like non-public or unpublished templates");

            if (await _likeRepository.HasUserLikedAsync(templateId, userId))
                throw new LikeAlreadyExistsException(templateId, userId);

            var like = new Like
            {
                TemplateId = templateId,
                UserId = userId
            };

            await _likeRepository.CreateAsync(like);
            return true;
        }

        public async Task<bool> RemoveLikeAsync(Guid userId, Guid templateId)
        {
            var existingLike = await _likeRepository.GetByTemplateAndUserAsync(templateId, userId);
            if (existingLike == null)
                return false;

            await _likeRepository.DeleteAsync(existingLike.Id);
            return true;
        }

        public async Task<bool> HasUserLikedAsync(Guid userId, Guid templateId)
        {
            return await _likeRepository.HasUserLikedAsync(templateId, userId);
        }

        public async Task<int> GetLikesCountAsync(Guid templateId)
        {
            return await _likeRepository.GetCountByTemplateAsync(templateId);
        }

        public async Task<PagedResult<LikeDto>> GetLikesPagedAsync(Guid templateId, int page, int pageSize)
        {
            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            var result = await _likeRepository.GetLikesByTemplatePagedAsync(templateId, page, pageSize);
            var likeDtos = result.Data.Select(MapToLikeDto).ToList();

            return new PagedResult<LikeDto>(likeDtos, result.Pagination.TotalCount, page, pageSize);
        }

        public async Task<PagedResult<TemplateListItemDto>> GetUserLikedTemplatesPagedAsync(Guid userId, int page, int pageSize)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            var result = await _likeRepository.GetLikesByUserPagedAsync(userId, page, pageSize);
            var templateDtos = new List<TemplateListItemDto>();

            foreach (var like in result.Data)
            {
                if (like.Template != null)
                {
                    templateDtos.Add(MapToTemplateListItemDto(like.Template));
                }
            }

            return new PagedResult<TemplateListItemDto>(templateDtos, result.Pagination.TotalCount, page, pageSize);
        }

        private static LikeDto MapToLikeDto(Like like)
        {
            return new LikeDto
            {
                Id = like.Id,
                TemplateId = like.TemplateId,
                UserId = like.UserId,
                UserName = like.User?.UserName ?? "Unknown User",
                CreatedAt = like.CreatedAt
            };
        }

        private static TemplateListItemDto MapToTemplateListItemDto(Template template)
        {
            return new TemplateListItemDto
            {
                Id = template.Id,
                Title = template.Title,
                Description = template.Description,
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

using FormFlow.Application.DTOs.Comments;
using FormFlow.Application.Interfaces;
using FormFlow.Domain.Exceptions;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Interfaces.Services;
using FormFlow.Domain.Models.General;
using FormFlow.Domain.Models.General.QuestionDetailsModels;
using FormFlow.Domain.Models.SearchService;
using System.Text.Json;

namespace FormFlow.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISearchService _searchService;

        public CommentService(
            ICommentRepository commentRepository,
            ITemplateRepository templateRepository,
            IUserRepository userRepository,
            ISearchService searchService)
        {
            _commentRepository = commentRepository;
            _templateRepository = templateRepository;
            _userRepository = userRepository;
            _searchService = searchService;
        }

        public async Task<CommentDto> AddCommentAsync(Guid userId, AddCommentRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            if (user.IsBlocked)
                throw new UserBlockedException(userId);

            var template = await _templateRepository.GetByIdAsync(request.TemplateId);
            if (template == null)
                throw new TemplateNotFoundException(request.TemplateId);

            if (!await CanUserCommentAsync(request.TemplateId, userId))
                throw new UnauthorizedAccessException("User cannot comment on this template");

            var comment = new Comment
            {
                TemplateId = request.TemplateId,
                UserId = userId,
                Content = request.Content.Trim()
            };

            var createdComment = await _commentRepository.CreateAsync(comment);

            await UpdateTemplateSearchIndexAsync(request.TemplateId);

            return await MapToCommentDtoAsync(createdComment, userId);
        }

        public async Task DeleteCommentAsync(Guid commentId, Guid userId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
                throw new CommentNotFoundException(commentId);

            if (!await CanDeleteCommentAsync(commentId, userId))
                throw new UnauthorizedAccessException("User cannot delete this comment");

            var templateId = comment.TemplateId;

            await _commentRepository.DeleteAsync(commentId);

            await UpdateTemplateSearchIndexAsync(templateId);
        }

        public async Task<CommentDto> GetCommentByIdAsync(Guid commentId)
        {
            var comment = await _commentRepository.GetWithUserAsync(commentId);
            if (comment == null)
                throw new CommentNotFoundException(commentId);

            return await MapToCommentDtoAsync(comment, null);
        }

        public async Task<bool> CommentExistsAsync(Guid commentId)
        {
            return await _commentRepository.ExistsAsync(commentId);
        }

        public async Task<PagedResult<CommentDto>> GetCommentsPagedAsync(Guid templateId, int page, int pageSize)
        {
            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            var result = await _commentRepository.GetCommentsByTemplatePagedAsync(templateId, page, pageSize);
            var commentDtos = new List<CommentDto>();

            foreach (var comment in result.Data)
            {
                commentDtos.Add(await MapToCommentDtoAsync(comment, null));
            }

            return new PagedResult<CommentDto>(commentDtos, result.Pagination.TotalCount, page, pageSize);
        }

        public async Task<List<CommentDto>> GetRecentCommentsAsync(Guid templateId, int count = 20)
        {
            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            var comments = await _commentRepository.GetRecentByTemplateAsync(templateId, count);
            var commentDtos = new List<CommentDto>();

            foreach (var comment in comments)
            {
                commentDtos.Add(await MapToCommentDtoAsync(comment, null));
            }

            return commentDtos;
        }

        public async Task<int> GetCommentsCountAsync(Guid templateId)
        {
            return await _commentRepository.GetCountByTemplateAsync(templateId);
        }

        public async Task<bool> CanDeleteCommentAsync(Guid commentId, Guid userId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null) return false;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            if (user.Role.HasFlag(UserRole.Admin)) return true;
            if (comment.UserId == userId) return true;

            return await _templateRepository.IsAuthorAsync(comment.TemplateId, userId);
        }

        public async Task<bool> CanUserCommentAsync(Guid templateId, Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.IsBlocked) return false;

            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null || !template.IsPublished) return false;

            return await _templateRepository.HasUserAccessAsync(templateId, userId);
        }

        private async Task<CommentDto> MapToCommentDtoAsync(Comment comment, Guid? currentUserId)
        {
            var canDelete = currentUserId.HasValue && await CanDeleteCommentAsync(comment.Id, currentUserId.Value);
            var isAuthor = currentUserId.HasValue && comment.UserId == currentUserId.Value;

            return new CommentDto
            {
                Id = comment.Id,
                TemplateId = comment.TemplateId,
                AuthorId = comment.UserId,
                AuthorName = comment.User?.UserName ?? "Unknown User",
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                CanDelete = canDelete,
                IsAuthor = isAuthor
            };
        }

        private async Task UpdateTemplateSearchIndexAsync(Guid templateId)
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
                .Aggregate("", (current, text) => current + " " + text).Trim() ?? "";

            var commentsText = template.Comments?
                .Where(c => !c.IsDeleted)
                .Select(c => c.Content)
                .Where(content => !string.IsNullOrWhiteSpace(content))
                .Aggregate("", (current, content) => current + " " + content).Trim() ?? "";

            return new TemplateSearchDocument
            {
                Id = template.Id,
                Title = template.Title,
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

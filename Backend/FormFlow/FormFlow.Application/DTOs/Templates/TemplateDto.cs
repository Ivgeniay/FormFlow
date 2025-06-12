using FormFlow.Application.DTOs.Tags;
using FormFlow.Application.DTOs.Users;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.DTOs.Templates
{
    public class TemplateDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public TemplateAccess AccessType { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int Version { get; set; }
        public bool IsCurrentVersion { get; set; }
        public bool IsPublished { get; set; }
        public bool IsArchived { get; set; }
        public Guid? BaseTemplateId { get; set; }
        public Guid? PreviousVersionId { get; set; }

        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
        public List<UserSearchDto> AllowedUsers { get; set; } = new List<UserSearchDto>();

        public int FormsCount { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public bool IsUserLiked { get; set; }
        public bool HasUserAccess { get; set; }
        public bool CanUserEdit { get; set; }
    }
}

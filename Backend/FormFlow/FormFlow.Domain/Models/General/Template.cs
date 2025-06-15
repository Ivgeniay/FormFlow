namespace FormFlow.Domain.Models.General
{
    public class Template
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AuthorId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid TopicId { get; set; }
        public string? ImageUrl { get; set; }
        public TemplateAccess AccessType { get; set; } = TemplateAccess.Public;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public bool IsArchived { get; set; } = false;
        public bool IsPublished { get; set; } = false;

        public int Version { get; set; } = 1;
        public bool IsCurrentVersion { get; set; } = true;
        public Guid? BaseTemplateId { get; set; }
        public Guid? PreviousVersionId { get; set; }


        public int LikesCount => Likes?.Count(l => !l.IsDeleted) ?? 0;
        public int FormsCount => Forms?.Count(f => !f.IsDeleted) ?? 0;
        public int CommentsCount => Comments?.Count(c => !c.IsDeleted) ?? 0;

        public User Author { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
        public List<Form> Forms { get; set; } = new List<Form>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Like> Likes { get; set; } = new List<Like>();
        public List<TemplateTag> Tags { get; set; } = new List<TemplateTag>();
        public List<TemplateAllowedUser> AllowedUsers { get; set; } = new List<TemplateAllowedUser>();
    }

    public enum TemplateAccess
    {
        Public = 1,
        Restricted = 2
    }
}

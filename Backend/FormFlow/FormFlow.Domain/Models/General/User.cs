namespace FormFlow.Domain.Models.General
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserName { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsBlocked { get; set; } = false;
        public bool IsDeleted { get; set; } = false;


        public GoogleAuth? GoogleAuth { get; set; }
        public EmailPasswordAuth? EmailAuth { get; set; }
        public List<UserContact> Contacts { get; set; } = new List<UserContact>();
        public List<Template> Templates { get; set; } = new List<Template>();
        public List<Form> Forms { get; set; } = new List<Form>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Like> Likes { get; set; } = new List<Like>();
    }

    [Flags]
    public enum UserRole
    {
        None = 0,
        User = 1,
        Admin = 2,
        Moderator = 4,
        SuperAdmin = 8
    }
}

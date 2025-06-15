namespace FormFlow.Domain.Models.General
{
    public class UserSettings
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid ColorThemeId { get; set; }
        public Guid LanguageId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        public User User { get; set; }
        public ColorTheme ColorTheme { get; set; }
        public Language Language { get; set; }
    }
}

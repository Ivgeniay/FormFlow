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

    public class ColorTheme
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string CssClass { get; set; } = string.Empty;
        public string PrimaryColor { get; set; } = "#000000"; 
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Language
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        // "en-US"
        public string Code { get; set; } = string.Empty;
        // "en"
        public string ShortCode { get; set; } = string.Empty;
        // "English"
        public string Name { get; set; } = string.Empty;
        // "United States"
        public string? IconURL { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

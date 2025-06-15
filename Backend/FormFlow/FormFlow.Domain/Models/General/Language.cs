namespace FormFlow.Domain.Models.General
{
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

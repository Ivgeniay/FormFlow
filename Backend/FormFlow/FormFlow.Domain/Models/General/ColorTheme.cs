namespace FormFlow.Domain.Models.General
{
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
}

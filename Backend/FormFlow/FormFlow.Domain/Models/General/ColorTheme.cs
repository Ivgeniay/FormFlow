namespace FormFlow.Domain.Models.General
{
    public class ColorTheme
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string CssClass { get; set; } = string.Empty;
        public string ColorVariables { get; set; } = string.Empty; 
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

namespace FormFlow.Domain.Models.General
{
    public class UserSettings
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string ColorTheme { get; set; } = "default";
        public Guid Language { get; set; }
    }

    public class Languages
    {
        public Guid Id;
        public string Region;
        public string Name;
        public string Code;
        public bool IsDefault;
        public bool IsActive;
    }
}

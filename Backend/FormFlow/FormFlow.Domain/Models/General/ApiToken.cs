namespace FormFlow.Domain.Models.General
{
    public class ApiToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        
        public User User { get; set; } = null!;
    }
}

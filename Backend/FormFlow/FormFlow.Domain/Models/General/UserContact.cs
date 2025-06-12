namespace FormFlow.Domain.Models.General
{
    public class UserContact
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ContactType Type { get; set; }
        public string Value { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public User User { get; set; }
    }

    public enum ContactType
    {
        Email = 1,
        Phone = 2,
        Address = 3,
        FacebookLink = 4,
        InstagramLink = 5
    }
}

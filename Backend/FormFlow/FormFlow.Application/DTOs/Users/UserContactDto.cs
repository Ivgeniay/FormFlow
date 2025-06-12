using FormFlow.Application.Interfaces;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.DTOs.Users
{
    public class UserContactDto
    {
        public Guid Id { get; set; }
        public ContactType Type { get; set; }
        public string Value { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

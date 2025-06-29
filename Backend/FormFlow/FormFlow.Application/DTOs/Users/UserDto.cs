using FormFlow.Application.DTOs.Templates;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.DTOs.Users
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsBlocked { get; set; }
        public int TemplatesCount { get; set; }
        public int FormsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<UserContactDto> Contacts { get; set; } = new List<UserContactDto>();
        public List<AuthMethodDto> AuthMethods { get; set; } = new List<AuthMethodDto>();
    }
}

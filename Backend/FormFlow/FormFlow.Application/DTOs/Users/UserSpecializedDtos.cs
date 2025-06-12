using FormFlow.Domain.Models.General;

namespace FormFlow.Application.DTOs.Users
{
    public class UserListItemDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PrimaryEmail { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TemplatesCount { get; set; }
        public int FormsCount { get; set; }
    }

    public class UserSearchDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PrimaryEmail { get; set; } = string.Empty;
    }
}

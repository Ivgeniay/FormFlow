using FormFlow.Domain.Models.Auth;
using FormFlow.Domain.Models.General;

namespace FormFlow.Infrastructure.Models
{
    public class CurrentUser
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public AuthType AuthType { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsAuthenticated { get; set; }

        public bool HasRole(UserRole role)
        {
            return Role.HasFlag(role);
        }

        public bool IsAdmin => HasRole(UserRole.Admin);
        public bool IsSuperAdmin => HasRole(UserRole.SuperAdmin);
        public bool IsModerator => HasRole(UserRole.Moderator);
    }
}

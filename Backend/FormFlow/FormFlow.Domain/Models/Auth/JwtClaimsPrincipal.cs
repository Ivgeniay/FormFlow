using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Models.Auth
{
    public class JwtClaimsPrincipal
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsBlocked { get; set; }
        public DateTime TokenIssuedAt { get; set; }
        public DateTime TokenExpiresAt { get; set; }
        public AuthType AuthType { get; set; }
    }
}

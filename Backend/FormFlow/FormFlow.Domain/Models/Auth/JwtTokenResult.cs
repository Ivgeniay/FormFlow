using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Models.Auth
{
    public class JwtTokenResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiry { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public Guid UserId { get; set; }
        public UserRole UserRole { get; set; }
        public AuthType AuthType { get; set; }
    }
}

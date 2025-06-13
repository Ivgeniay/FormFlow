namespace FormFlow.Domain.Models.Auth
{
    public class ExternalAuthInfo
    {
        public AuthType Provider { get; set; }
        public string ExternalRefreshToken { get; set; } = string.Empty;
        public DateTime ExternalTokenExpiry { get; set; }
        public string ExternalUserId { get; set; } = string.Empty;
    }
}

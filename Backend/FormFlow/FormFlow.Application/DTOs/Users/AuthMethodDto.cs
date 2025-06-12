namespace FormFlow.Application.DTOs.Users
{
    public abstract class AuthMethodDto
    {
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class EmailAuthMethodDto : AuthMethodDto
    {
        public string Email { get; set; } = string.Empty;
    }


    public class GoogleAuthMethodDto : AuthMethodDto
    {
        public string GoogleId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime TokenExpiry { get; set; }
    }
}

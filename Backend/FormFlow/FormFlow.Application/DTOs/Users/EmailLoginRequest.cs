namespace FormFlow.Application.DTOs.Users
{
    public class EmailLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

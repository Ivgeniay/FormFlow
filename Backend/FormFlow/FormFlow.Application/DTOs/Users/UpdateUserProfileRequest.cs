namespace FormFlow.Application.DTOs.Users
{
    public class UpdateUserProfileRequest
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}

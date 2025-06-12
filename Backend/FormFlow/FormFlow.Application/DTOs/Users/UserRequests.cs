using FormFlow.Domain.Models.General;

namespace FormFlow.Application.DTOs.Users
{
    public class RegisterUserRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class EmailLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class GoogleLoginRequest
    {
        public string GoogleToken { get; set; } = string.Empty;
    }

    public class UpdateUserProfileRequest
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
    }

    public class AddContactRequest
    {
        public ContactType Type { get; set; }
        public string Value { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }

    public class UpdateContactRequest
    {
        public Guid Id { get; set; }
        public ContactType Type { get; set; }
        public string Value { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}

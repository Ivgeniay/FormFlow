using FormFlow.Application.DTOs.Users;
using FormFlow.Application.Services;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticationResult> RegisterUserAsync(RegisterUserRequest request);
        Task<AuthenticationResult> AuthenticateWithEmailAsync(EmailLoginRequest request);
        Task<AuthenticationResult> AuthenticateWithGoogleAsync(GoogleLoginRequest request);
        Task<RefreshTokenResult> RefreshTokenAsync(RefreshTokenRequest request);
        Task<LogoutResult> LogoutAsync(Guid userId, LogoutRequest request);
        Task<ValidateTokenResult> ValidateTokenAsync(ValidateTokenRequest request);
        Task<ChangeRoleResult> ChangeUserRoleAsync(ChangeRoleRequest request, Guid promotingUserId);

        Task<List<UserContactDto>> GetUserContactsAsync(Guid userId);

        Task<UserDto> GetUserByIdAsync(Guid id);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<UserDto> UpdateUserProfileAsync(UpdateUserProfileRequest request);
        Task DeleteUserAsync(Guid id);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UserNameExistsAsync(string userName);

        Task<PagedResult<UserDto>> GetUsersPagedAsync(int page, int pageSize);
        Task<List<UserDto>> SearchUsersAsync(string query, int limit = 10);

        Task BlockUserAsync(Guid userId, Guid adminId);
        Task UnblockUserAsync(Guid userId, Guid adminId);
        bool CanChangeUserRole(UserRole promotingUserRole, UserRole targetUserCurrentRole, UserRole targetUserNewRole, bool isSelfChange);

        Task<UserDto> AddUserContactAsync(Guid userId, AddContactRequest request);
        Task RemoveUserContactAsync(Guid userId, Guid contactId);
        Task<UserDto> UpdateUserContactAsync(Guid userId, UpdateContactRequest request);
    }

    
}

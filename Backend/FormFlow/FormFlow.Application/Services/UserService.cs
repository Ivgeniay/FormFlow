using FormFlow.Application.DTOs;
using FormFlow.Application.DTOs.Users;
using FormFlow.Application.Interfaces;
using FormFlow.Domain.Exceptions;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> RegisterUserAsync(RegisterUserRequest request)
        {
            if (await _userRepository.EmailExistsAsync(request.Email))
                throw new EmailAlreadyExistsException(request.Email);

            if (await _userRepository.UserNameExistsAsync(request.UserName))
                throw new UserNameAlreadyExistsException(request.UserName);

            var user = new User
            {
                UserName = request.UserName,
                Role = UserRole.User
            };

            var emailAuth = new EmailPasswordAuth
            {
                UserId = user.Id,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            user.EmailAuth = emailAuth;

            var primaryContact = new UserContact
            {
                UserId = user.Id,
                Type = ContactType.Email,
                Value = request.Email,
                IsPrimary = true
            };

            user.Contacts.Add(primaryContact);

            await _userRepository.CreateUserWithAuthAsync(user);

            return DTOMapper.MapToUserDto(user);
        }

        public async Task<UserDto> AuthenticateWithEmailAsync(EmailLoginRequest request)
        {
            var user = await _userRepository.GetForAuthenticationAsync(request.Email);

            if (user == null || user.EmailAuth == null)
                throw new UserNotFoundException(request.Email);

            if (user.IsBlocked)
                throw new UserBlockedException(user.Id);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.EmailAuth.PasswordHash))
                throw new UserNotFoundException(request.Email);

            return DTOMapper.MapToUserDto(user);
        }

        public async Task<UserDto> AuthenticateWithGoogleAsync(GoogleLoginRequest request)
        {
            throw new NotImplementedException("Google authentication not implemented yet");
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetWithContactsAsync(id);
            if (user == null)
                throw new UserNotFoundException(id);

            var userWithAuth = await _userRepository.GetWithAuthMethodsAsync(id);
            if (userWithAuth != null)
            {
                user.EmailAuth = userWithAuth.EmailAuth;
                user.GoogleAuth = userWithAuth.GoogleAuth;
            }

            return DTOMapper.MapToUserDto(user);
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new UserNotFoundException(email);

            return DTOMapper.MapToUserDto(user);
        }

        public async Task<UserDto> UpdateUserProfileAsync(UpdateUserProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
                throw new UserNotFoundException(request.Id);

            if (user.UserName != request.UserName && await _userRepository.UserNameExistsAsync(request.UserName))
                throw new UserNameAlreadyExistsException(request.UserName);

            user.UserName = request.UserName;

            await _userRepository.UpdateAsync(user);

            return DTOMapper.MapToUserDto(user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new UserNotFoundException(id);

            await _userRepository.DeleteAsync(id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email);
        }

        public async Task<bool> UserNameExistsAsync(string userName)
        {
            return await _userRepository.UserNameExistsAsync(userName);
        }

        public async Task<PagedResult<UserDto>> GetUsersPagedAsync(int page, int pageSize)
        {
            var result = await _userRepository.GetUsersWithContactsPagedAsync(page, pageSize);
            var userDtos = result.Data.Select(DTOMapper.MapToUserListItemDto).Cast<UserDto>().ToList();
            return new PagedResult<UserDto>(userDtos, result.Pagination.TotalCount, page, pageSize);
        }

        public async Task<List<UserDto>> SearchUsersAsync(string query, int limit = 10)
        {
            var users = await _userRepository.GetUsersForSearchAsync(query, limit);
            return users.Select(DTOMapper.MapToUserSearchDto).Cast<UserDto>().ToList();
        }

        public async Task BlockUserAsync(Guid userId, Guid adminId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            var admin = await _userRepository.GetByIdAsync(adminId);
            if (admin == null || !admin.Role.HasFlag(UserRole.Admin))
                throw new UnauthorizedAccessException("Only admins can block users");

            user.IsBlocked = true;
            await _userRepository.UpdateAsync(user);
        }

        public async Task UnblockUserAsync(Guid userId, Guid adminId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            var admin = await _userRepository.GetByIdAsync(adminId);
            if (admin == null || !admin.Role.HasFlag(UserRole.Admin))
                throw new UnauthorizedAccessException("Only admins can unblock users");

            user.IsBlocked = false;
            await _userRepository.UpdateAsync(user);
        }

        public async Task SetUserRoleAsync(Guid userId, UserRole role, Guid promotingUserId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            var promotingUser = await _userRepository.GetByIdAsync(promotingUserId);
            if (promotingUser == null)
                throw new UserNotFoundException(promotingUserId);

            bool isSelfChange = userId == promotingUserId;

            if (!CanChangeUserRole(promotingUser.Role, user.Role, role, isSelfChange))
                throw new UnauthorizedAccessException("Insufficient permissions to change user role");

            user.Role = role;
            await _userRepository.UpdateAsync(user);
        }

        public bool CanChangeUserRole(UserRole promotingUserRole, UserRole targetUserCurrentRole, UserRole targetUserNewRole, bool isSelfChange)
        {
            if (isSelfChange && targetUserCurrentRole.HasFlag(UserRole.Admin) && !targetUserNewRole.HasFlag(UserRole.Admin))
                return true;

            if (!promotingUserRole.HasFlag(UserRole.Admin))
                return false;

            if (targetUserCurrentRole.HasFlag(UserRole.SuperAdmin))
                return false;

            if (targetUserNewRole.HasFlag(UserRole.SuperAdmin) && !promotingUserRole.HasFlag(UserRole.SuperAdmin))
                return false;

            return true;
        }

        public async Task<UserDto> AddUserContactAsync(Guid userId, AddContactRequest request)
        {
            var user = await _userRepository.GetWithContactsAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            if (request.IsPrimary)
            {
                foreach (var contact in user.Contacts.Where(c => c.Type == request.Type))
                {
                    contact.IsPrimary = false;
                }
            }

            var newContact = new UserContact
            {
                UserId = userId,
                Type = request.Type,
                Value = request.Value,
                IsPrimary = request.IsPrimary
            };

            user.Contacts.Add(newContact);
            await _userRepository.UpdateAsync(user);

            return DTOMapper.MapToUserDto(user);
        }

        public async Task RemoveUserContactAsync(Guid userId, Guid contactId)
        {
            var user = await _userRepository.GetWithContactsAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            var contact = user.Contacts.FirstOrDefault(c => c.Id == contactId);
            if (contact != null)
            {
                user.Contacts.Remove(contact);
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task<UserDto> UpdateUserContactAsync(Guid userId, UpdateContactRequest request)
        {
            var user = await _userRepository.GetWithContactsAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            var contact = user.Contacts.FirstOrDefault(c => c.Id == request.Id);
            if (contact == null)
                throw new ArgumentException("Contact not found");

            if (request.IsPrimary && contact.Type != request.Type)
            {
                foreach (var c in user.Contacts.Where(c => c.Type == request.Type))
                {
                    c.IsPrimary = false;
                }
            }

            contact.Type = request.Type;
            contact.Value = request.Value;
            contact.IsPrimary = request.IsPrimary;
            contact.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return DTOMapper.MapToUserDto(user);
        }

        public async Task SetPrimaryContactAsync(Guid userId, Guid contactId)
        {
            var user = await _userRepository.GetWithContactsAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            var contact = user.Contacts.FirstOrDefault(c => c.Id == contactId);
            if (contact == null)
                throw new ArgumentException("Contact not found");

            foreach (var c in user.Contacts.Where(c => c.Type == contact.Type))
            {
                c.IsPrimary = false;
            }

            contact.IsPrimary = true;
            await _userRepository.UpdateAsync(user);
        }

    }
}

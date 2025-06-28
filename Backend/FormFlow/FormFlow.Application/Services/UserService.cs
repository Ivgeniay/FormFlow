using FormFlow.Application.DTOs;
using FormFlow.Application.DTOs.Users;
using FormFlow.Application.Interfaces;
using FormFlow.Domain.Exceptions;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Interfaces.Services;
using FormFlow.Domain.Models.Auth;
using FormFlow.Domain.Models.General;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FormFlow.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly IGoogleAuthRepository _googleAuthRepository;
        private readonly IUserContactRepository _userContactRepository;
        private readonly IEmailAuthRepository _emailAuthRepository;

        public UserService(IUserRepository userRepository, 
            IJwtService jwtService, 
            IGoogleAuthService googleAuthService,
            IUserSettingsRepository userSettingsRepository, 
            IGoogleAuthRepository googleAuthRepository,
            IUserContactRepository userContactRepository,
        IEmailAuthRepository emailAuthRepository
            )
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _googleAuthService = googleAuthService;
            _userSettingsRepository = userSettingsRepository;
            _googleAuthRepository = googleAuthRepository;
            _userContactRepository = userContactRepository;
            _emailAuthRepository = emailAuthRepository;
        }

        public async Task<AuthenticationResult> RegisterUserAsync(RegisterUserRequest request)
        {
            try
            {
                if (await _userRepository.EmailExistsAsync(request.Email))
                    return new AuthenticationResult { IsSuccess = false, ErrorMessage = "Email already exists" };

                var existingGoogleAuth = await _googleAuthRepository.GetByEmailAsync(request.Email);
                if (existingGoogleAuth != null)
                {
                    var existingUser = await _userRepository.GetByIdAsync(existingGoogleAuth.UserId);
                    if (existingUser == null)
                        return new AuthenticationResult { IsSuccess = false, ErrorMessage = "User not found" };

                    if (existingUser.IsBlocked)
                        return new AuthenticationResult { IsSuccess = false, ErrorMessage = "User account is blocked" };

                    await _emailAuthRepository.CreateAsync(new EmailPasswordAuth
                    {
                        UserId = existingUser.Id,
                        Email = request.Email,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
                    });

                    //var emailAuth = new EmailPasswordAuth
                    //{
                    //    UserId = existingUser.Id,
                    //    Email = request.Email,
                    //    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
                    //};

                    var tokenResult = await _jwtService.GenerateTokenAsync(existingUser, AuthType.Internal);

                    return new AuthenticationResult
                    {
                        User = DTOMapper.MapToUserDto(existingUser),
                        AccessToken = tokenResult.AccessToken,
                        RefreshToken = tokenResult.RefreshToken,
                        AccessTokenExpiry = tokenResult.AccessTokenExpiry,
                        RefreshTokenExpiry = tokenResult.RefreshTokenExpiry,
                        AuthType = AuthType.Internal,
                        IsSuccess = true
                    };
                }

                //if (await _userRepository.UserNameExistsAsync(request.UserName))
                //    return new AuthenticationResult { IsSuccess = false, ErrorMessage = "Username already exists" };

                var user = new User
                {
                    UserName = request.UserName,
                    Role = UserRole.User
                };
                User userInDb = await _userRepository.CreateAsync(user);

                await _emailAuthRepository.CreateAsync(new EmailPasswordAuth
                {
                    UserId = userInDb.Id,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
                });


                await _userContactRepository.CreateAsync(new UserContact
                {
                    UserId = user.Id,
                    Type = ContactType.Email,
                    Value = request.Email,
                    IsPrimary = true
                });

                if (!await _userSettingsRepository.ExistsByUserIdAsync(userInDb.Id))
                {
                    await _userSettingsRepository.CreateDefaultForUserAsync(userInDb.Id);
                }

                var newUserTokenResult = await _jwtService.GenerateTokenAsync(user, AuthType.Internal);

                return new AuthenticationResult
                {
                    User = DTOMapper.MapToUserDto(user),
                    AccessToken = newUserTokenResult.AccessToken,
                    RefreshToken = newUserTokenResult.RefreshToken,
                    AccessTokenExpiry = newUserTokenResult.AccessTokenExpiry,
                    RefreshTokenExpiry = newUserTokenResult.RefreshTokenExpiry,
                    AuthType = AuthType.Internal,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new AuthenticationResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        //public async Task<AuthenticationResult> RegisterUserAsync(RegisterUserRequest request)
        //{
        //    try
        //    {
        //        if (await _userRepository.EmailExistsAsync(request.Email))
        //            return new AuthenticationResult { IsSuccess = false, ErrorMessage = "Email already exists" };

        //        if (await _userRepository.UserNameExistsAsync(request.UserName))
        //            return new AuthenticationResult { IsSuccess = false, ErrorMessage = "Username already exists" };

        //        var user = new User
        //        {
        //            UserName = request.UserName,
        //            Role = UserRole.User
        //        };

        //        var emailAuth = new EmailPasswordAuth
        //        {
        //            UserId = user.Id,
        //            Email = request.Email,
        //            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        //        };

        //        user.EmailAuth = emailAuth;

        //        User userInDb = await _userRepository.CreateUserWithAuthAsync(user);

        //        await _userContactRepository.CreateAsync(new UserContact
        //        {
        //            UserId = user.Id,
        //            Type = ContactType.Email,
        //            Value = request.Email,
        //            IsPrimary = true
        //        });


        //        if (!await _userSettingsRepository.ExistsByUserIdAsync(userInDb.Id))
        //        {
        //            await _userSettingsRepository.CreateDefaultForUserAsync(userInDb.Id);
        //        }

        //        var tokenResult = await _jwtService.GenerateTokenAsync(user, AuthType.Internal);

        //        return new AuthenticationResult
        //        {
        //            User = DTOMapper.MapToUserDto(user),
        //            AccessToken = tokenResult.AccessToken,
        //            RefreshToken = tokenResult.RefreshToken,
        //            AccessTokenExpiry = tokenResult.AccessTokenExpiry,
        //            RefreshTokenExpiry = tokenResult.RefreshTokenExpiry,
        //            AuthType = AuthType.Internal,
        //            IsSuccess = true
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new AuthenticationResult { IsSuccess = false, ErrorMessage = ex.Message };
        //    }
        //}

        public async Task<AuthenticationResult> AuthenticateWithEmailAsync(EmailLoginRequest request)
        {
            try
            {
                var user = await _userRepository.GetForAuthenticationAsync(request.Email);

                if (user == null || user.EmailAuth == null)
                    return new AuthenticationResult { IsSuccess = false, ErrorMessage = "Invalid email or password" };

                if (user.IsBlocked)
                    return new AuthenticationResult { IsSuccess = false, ErrorMessage = "User account is blocked" };

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.EmailAuth.PasswordHash))
                    return new AuthenticationResult { IsSuccess = false, ErrorMessage = "Invalid email or password" };

                var tokenResult = await _jwtService.GenerateTokenAsync(user, AuthType.Internal);

                return new AuthenticationResult
                {
                    User = DTOMapper.MapToUserDto(user),
                    AccessToken = tokenResult.AccessToken,
                    RefreshToken = tokenResult.RefreshToken,
                    AccessTokenExpiry = tokenResult.AccessTokenExpiry,
                    RefreshTokenExpiry = tokenResult.RefreshTokenExpiry,
                    AuthType = AuthType.Internal,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new AuthenticationResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<AuthenticationResult> AuthenticateWithGoogleAsync(GoogleLoginRequest request)
        {
            try
            {
                var googleUserInfo = await _googleAuthService.GetUserInfoAsync(request.Code);
                var existingGoogleAuth = await _googleAuthRepository.GetByGoogleIdAsync(googleUserInfo.GoogleId);

                if (existingGoogleAuth != null)
                {
                    var user = await _userRepository.GetByIdAsync(existingGoogleAuth.UserId);

                    if (user == null)
                        return new AuthenticationResult { IsSuccess = false, ErrorMessage = "User not found" };

                    if (user.IsBlocked)
                        return new AuthenticationResult { IsSuccess = false, ErrorMessage = "User account is blocked" };

                    if (existingGoogleAuth.Email != googleUserInfo.Email || 
                        existingGoogleAuth.PictureUrl != googleUserInfo.Picture)
                    {
                        existingGoogleAuth.Email = googleUserInfo.Email;
                        existingGoogleAuth.PictureUrl = googleUserInfo.Picture;
                        await _googleAuthRepository.UpdateAsync(existingGoogleAuth);
                    }

                    var tokenResult = await _jwtService.GenerateTokenAsync(user, AuthType.Google);

                    return new AuthenticationResult
                    {
                        User = DTOMapper.MapToUserDto(user),
                        AccessToken = tokenResult.AccessToken,
                        RefreshToken = tokenResult.RefreshToken,
                        AccessTokenExpiry = tokenResult.AccessTokenExpiry,
                        RefreshTokenExpiry = tokenResult.RefreshTokenExpiry,
                        AuthType = AuthType.Google,
                        IsSuccess = true
                    };
                }

                var existingUser = await _userRepository.GetByEmailAsync(googleUserInfo.Email);

                if (existingUser != null)
                {
                    if (existingUser.IsBlocked)
                        return new AuthenticationResult { IsSuccess = false, ErrorMessage = "User account is blocked" };

                    var existingGoogleByEmail = await _googleAuthRepository.GetByEmailAsync(googleUserInfo.Email);
                    if (existingGoogleByEmail != null)
                        return new AuthenticationResult { IsSuccess = false, ErrorMessage = "This Google account is already linked to another user" };

                    await _googleAuthRepository.CreateAsync(new GoogleAuth
                    {
                        UserId = existingUser.Id,
                        GoogleId = googleUserInfo.GoogleId,
                        Email = googleUserInfo.Email,
                        PictureUrl = googleUserInfo.Picture
                    });

                    var tokenResult = await _jwtService.GenerateTokenAsync(existingUser, AuthType.Google);

                    return new AuthenticationResult
                    {
                        User = DTOMapper.MapToUserDto(existingUser),
                        AccessToken = tokenResult.AccessToken,
                        RefreshToken = tokenResult.RefreshToken,
                        AccessTokenExpiry = tokenResult.AccessTokenExpiry,
                        RefreshTokenExpiry = tokenResult.RefreshTokenExpiry,
                        AuthType = AuthType.Google,
                        IsSuccess = true
                    };
                }

                var newUser = new User
                {
                    UserName = googleUserInfo.Name,
                    Role = UserRole.User
                };

                var userDb = await _userRepository.CreateUserWithAuthAsync(newUser);

                await _userContactRepository.CreateAsync(new UserContact
                {
                    UserId = userDb.Id,
                    Type = ContactType.Email,
                    Value = googleUserInfo.Email,
                    IsPrimary = true
                });

                await _googleAuthRepository.CreateAsync(new GoogleAuth
                {
                    UserId = userDb.Id,
                    GoogleId = googleUserInfo.GoogleId,
                    Email = googleUserInfo.Email,
                    PictureUrl = googleUserInfo.Picture,
                });

                var newUserTokenResult = await _jwtService.GenerateTokenAsync(userDb, AuthType.Google);

                return new AuthenticationResult
                {
                    User = DTOMapper.MapToUserDto(userDb),
                    AccessToken = newUserTokenResult.AccessToken,
                    RefreshToken = newUserTokenResult.RefreshToken,
                    AccessTokenExpiry = newUserTokenResult.AccessTokenExpiry,
                    RefreshTokenExpiry = newUserTokenResult.RefreshTokenExpiry,
                    AuthType = AuthType.Google,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new AuthenticationResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<RefreshTokenResult> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                var tokenResult = await _jwtService.RefreshTokenAsync(request.RefreshToken);

                return new RefreshTokenResult
                {
                    AccessToken = tokenResult.AccessToken,
                    RefreshToken = tokenResult.RefreshToken,
                    AccessTokenExpiry = tokenResult.AccessTokenExpiry,
                    RefreshTokenExpiry = tokenResult.RefreshTokenExpiry,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new RefreshTokenResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<LogoutResult> LogoutAsync(Guid userId, LogoutRequest request)
        {
            try
            {
                if (request.LogoutFromAllDevices)
                {
                    await _jwtService.RevokeAllUserTokensAsync(userId);
                }
                else if (!string.IsNullOrEmpty(request.RefreshToken))
                {
                    await _jwtService.RevokeTokenAsync(request.RefreshToken);
                }

                return new LogoutResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new LogoutResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<ValidateTokenResult> ValidateTokenAsync(ValidateTokenRequest request)
        {
            try
            {
                var isValid = await _jwtService.ValidateTokenAsync(request.Token);
                if (!isValid)
                    return new ValidateTokenResult { IsValid = false, ErrorMessage = "Invalid token" };

                var claims = await _jwtService.GetClaimsFromTokenAsync(request.Token);

                return new ValidateTokenResult
                {
                    IsValid = true,
                    Claims = claims
                };
            }
            catch (Exception ex)
            {
                return new ValidateTokenResult { IsValid = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<ChangeRoleResult> ChangeUserRoleAsync(ChangeRoleRequest request, Guid promotingUserId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                    return new ChangeRoleResult { IsSuccess = false, ErrorMessage = "User not found" };

                var promotingUser = await _userRepository.GetByIdAsync(promotingUserId);
                if (promotingUser == null)
                    return new ChangeRoleResult { IsSuccess = false, ErrorMessage = "Promoting user not found" };

                bool isSelfChange = request.UserId == promotingUserId;

                if (!CanChangeUserRole(promotingUser.Role, user.Role, request.NewRole, isSelfChange))
                    return new ChangeRoleResult { IsSuccess = false, ErrorMessage = "Insufficient permissions to change user role" };

                user.Role = request.NewRole;
                await _userRepository.UpdateAsync(user);

                var tokenResult = await _jwtService.RegenerateTokenForRoleChangeAsync(request.UserId, request.AuthType);

                return new ChangeRoleResult
                {
                    User = DTOMapper.MapToUserDto(user),
                    AccessToken = tokenResult.AccessToken,
                    RefreshToken = tokenResult.RefreshToken,
                    AccessTokenExpiry = tokenResult.AccessTokenExpiry,
                    RefreshTokenExpiry = tokenResult.RefreshTokenExpiry,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new ChangeRoleResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<List<UserContactDto>> GetUserContactsAsync(Guid userId)
        {
            return await Task.FromResult<List<UserContactDto>>(new List<UserContactDto>()); 
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
            var userDtos = result.Data.Select(DTOMapper.MapToUserDto).Cast<UserDto>().ToList();
            return new PagedResult<UserDto>(userDtos, result.Pagination.TotalCount, page, pageSize);
        }

        public async Task<List<UserDto>> SearchUsersAsync(string query, int limit = 10)
        {
            var users = await _userRepository.GetUsersForSearchAsync(query, limit);
            return users.Select(DTOMapper.MapToUserDto).Cast<UserDto>().ToList();
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

            await _jwtService.RevokeAllUserTokensAsync(userId);
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
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            if (request.IsPrimary)
            {
                var consact = await _userContactRepository.GetPrimaryContactAsync(userId);
                if (consact != null)
                {
                    consact.IsPrimary = false;
                    await _userContactRepository.UpdateAsync(consact);
                }
            }

            var newContact = new UserContact
            {
                UserId = userId,
                Type = request.Type,
                Value = request.Value,
                IsPrimary = request.IsPrimary
            };

            await _userContactRepository.CreateAsync(newContact);

            return DTOMapper.MapToUserDto(user);
        }

        public async Task RemoveUserContactAsync(Guid userId, Guid contactId)
        {
            var user = await _userRepository.GetWithContactsAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            var contact = user.Contacts.FirstOrDefault(c => c.Id == contactId);
            if (contact == null || contact.UserId != userId)
                throw new ArgumentException("Contact not found or doesn't belong to user");

            await _userContactRepository.DeleteAsync(contactId);
        }

        public async Task<UserDto> UpdateUserContactAsync(Guid userId, UpdateContactRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            var contact = await _userContactRepository.GetByIdAsync(request.Id);
            if (contact == null || contact.UserId != userId)
                throw new ArgumentException("Contact not found or doesn't belong to user");

            if (request.IsPrimary)
            {
                var consact = await _userContactRepository.GetPrimaryContactAsync(userId);
                if (consact != null)
                {
                    consact.IsPrimary = false;
                    await _userContactRepository.UpdateAsync(consact);
                }
            }

            contact.Type = request.Type;
            contact.Value = request.Value;
            contact.IsPrimary = request.IsPrimary;

            await _userContactRepository.UpdateAsync(contact);

            return DTOMapper.MapToUserDto(user);
        }

        [HttpPost("promote-to-admin")]
        public async Task<AuthenticationResult> PromoteToRole(Guid userId, UserRole role)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new UserNotFoundException(userId);

                if (user.IsBlocked)
                    throw new UserBlockedException(userId);

                if (user.Role.HasFlag(role))
                    return new AuthenticationResult { IsSuccess = false, ErrorMessage = $"User is already an {Enum.GetName<UserRole>(role)}" };

                user.Role = role;
                await _userRepository.UpdateAsync(user);

                await _jwtService.RevokeAllUserTokensAsync(userId);

                var tokenResult = await _jwtService.GenerateTokenAsync(user, user.EmailAuth != null ? AuthType.Internal : AuthType.Google);

                return new AuthenticationResult
                {
                    User = DTOMapper.MapToUserDto(user),
                    AccessToken = tokenResult.AccessToken,
                    RefreshToken = tokenResult.RefreshToken,
                    AccessTokenExpiry = tokenResult.AccessTokenExpiry,
                    RefreshTokenExpiry = tokenResult.RefreshTokenExpiry,
                    AuthType = tokenResult.AuthType,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new AuthenticationResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

    }

    public class AuthenticationResult
    {
        public UserDto User { get; set; } = new UserDto();
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiry { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public AuthType AuthType { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshTokenResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiry { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }

    public class LogoutRequest
    {
        public string? RefreshToken { get; set; }
        public bool LogoutFromAllDevices { get; set; } = false;
    }

    public class LogoutResult
    {
        public bool IsSuccess { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }

    public class ChangeRoleRequest
    {
        public Guid UserId { get; set; }
        public UserRole NewRole { get; set; }
        public AuthType AuthType { get; set; } = AuthType.Internal;
    }

    public class ChangeRoleResult
    {
        public UserDto User { get; set; } = new UserDto();
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiry { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }

    public class ValidateTokenRequest
    {
        public string Token { get; set; } = string.Empty;
    }

    public class ValidateTokenResult
    {
        public bool IsValid { get; set; }
        public JwtClaimsPrincipal? Claims { get; set; }
        public string? ErrorMessage { get; set; }
    }

    //public class EmailLoginRequest
    //{
    //    public string Email { get; set; } = string.Empty;
    //    public string Password { get; set; } = string.Empty;
    //}

    //public class GoogleLoginRequest
    //{
    //    public string GoogleToken { get; set; } = string.Empty;
    //}

    //public class RegisterUserRequest
    //{
    //    public string UserName { get; set; } = string.Empty;
    //    public string Email { get; set; } = string.Empty;
    //    public string Password { get; set; } = string.Empty;
    //}

}

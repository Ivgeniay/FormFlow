using FormFlow.Application.DTOs.Users;
using FormFlow.Application.Interfaces;
using FormFlow.Application.Services;
using FormFlow.Domain.Interfaces.Services.Jwt;
using FormFlow.Domain.Models.Auth;
using FormFlow.Domain.Models.General;
using FormFlow.WebApi.Common.Attributes;
using FormFlow.WebApi.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public UserController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpGet]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (pageSize > 100) pageSize = 100;
                if (pageSize < 1) pageSize = 20;
                if (page < 1) page = 1;

                var result = await _userService.GetUsersPagedAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers(
            [FromQuery] string q,
            [FromQuery] int limit = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                    return Ok(new List<UserDto>());

                if (limit > 50) limit = 50;
                if (limit < 1) limit = 10;

                var users = await _userService.SearchUsersAsync(q, limit);
                var result = users.Select(u => new {
                    id = u.Id,
                    userName = u.UserName,
                    //primaryEmail = u.Contacts?.FirstOrDefault(e => e.Type == ContactType.Email)?.Value
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            try
            {
                var currentUserId = this.GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                //if (currentUserId != id && !this.IsCurrentUserInRole(UserRole.Admin))
                //    return Forbid("You can only view your own profile");

                var user = await _userService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("many")]
        public async Task<IActionResult> GetUsers([FromQuery] Guid[] ids)
        {
            try
            {
                var currentUserId = this.GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                //if (currentUserId != id && !this.IsCurrentUserInRole(UserRole.Admin))
                //    return Forbid("You can only view your own profile");

                var user = await _userService.GetUsersByIdAsync(ids);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserProfile(Guid id, [FromBody] UpdateUserProfileRequest request)
        {
            try
            {
                var currentUserId = this.GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (currentUserId != id && !this.IsCurrentUserInRole(UserRole.Admin))
                    return Forbid("You can only update your own profile");

                request.Id = id;
                var user = await _userService.UpdateUserProfileAsync(request);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/block")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> BlockUser(Guid id)
        {
            try
            {
                var adminId = this.GetCurrentUserId();
                if (adminId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (adminId == id)
                    return BadRequest(new { message = "You cannot block yourself" });

                await _userService.BlockUserAsync(id, adminId.Value);
                return Ok(new { message = "User blocked successfully", userId = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/unblock")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> UnblockUser(Guid id)
        {
            try
            {
                var adminId = this.GetCurrentUserId();
                if (adminId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                await _userService.UnblockUserAsync(id, adminId.Value);
                return Ok(new { message = "User unblocked successfully", userId = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/role")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> ChangeUserRole(Guid id, [FromBody] ChangeRoleRequest request)
        {
            try
            {
                var promotingUserId = this.GetCurrentUserId();
                if (promotingUserId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                request.UserId = id;
                var result = await _userService.ChangeUserRoleAsync(request, promotingUserId.Value);

                if (!result.IsSuccess)
                    return BadRequest(new { message = result.ErrorMessage });

                return Ok(new
                {
                    message = "User role changed successfully",
                    user = result.User,
                    //accessToken = result.AccessToken,
                    //refreshToken = result.RefreshToken,
                    //accessTokenExpiry = result.AccessTokenExpiry,
                    //refreshTokenExpiry = result.RefreshTokenExpiry
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/toggle-admin")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> ToggleAdminRole(Guid id)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (userId.Value != id)
                {
                    var user = await _userService.ToggleAdminUserRole(id);

                    return Ok(new
                    {
                        message = "Role updated successfully",
                        data = user,
                    });
                }

                var currentUser = this.GetCurrentUser();
                if (currentUser == null)
                    return Unauthorized(new { message = "Invalid user context" });
                var result = await _userService.PromoteToRole(userId.Value, currentUser.HasRole(UserRole.Admin) ? UserRole.User: UserRole.Admin);

                if (!result.IsSuccess)
                    return BadRequest(new { message = result.ErrorMessage });

                return Ok(new
                {
                    message = "User promoted to admin successfully",
                    user = result.User,
                    accessToken = result.AccessToken,
                    refreshToken = result.RefreshToken,
                    accessTokenExpiry = result.AccessTokenExpiry,
                    refreshTokenExpiry = result.RefreshTokenExpiry,
                    authType = result.AuthType.ToString()
                });

            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var adminId = this.GetCurrentUserId();
                if (adminId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (adminId == id)
                    return BadRequest(new { message = "You cannot delete yourself" });

                await _userService.DeleteUserAsync(id);
                return Ok(new { message = "User deleted successfully", userId = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("stats")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetUserStats()
        {
            try
            {
                var allUsers = await _userService.GetUsersPagedAsync(1, int.MaxValue);
                var today = DateTime.UtcNow.Date;
                var lastWeek = today.AddDays(-7);
                var lastMonth = today.AddMonths(-1);

                var stats = new
                {
                    TotalUsers = allUsers.Pagination.TotalCount,
                    ActiveUsers = allUsers.Data.Count(u => !u.IsBlocked),
                    BlockedUsers = allUsers.Data.Count(u => u.IsBlocked),
                    AdminUsers = allUsers.Data.Count(u => u.Role.HasFlag(UserRole.Admin)),
                    RegularUsers = allUsers.Data.Count(u => u.Role == UserRole.User),
                    NewUsersThisWeek = allUsers.Data.Count(u => u.CreatedAt >= lastWeek),
                    NewUsersThisMonth = allUsers.Data.Count(u => u.CreatedAt >= lastMonth),
                    UsersByRole = allUsers.Data.GroupBy(u => u.Role).ToDictionary(g => g.Key.ToString(), g => g.Count()),
                    GeneratedAt = DateTime.UtcNow
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var user = await _userService.GetUserByIdAsync(userId.Value);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("me/contacts")]
        public async Task<IActionResult> GetContacts()
        {
            try
            {
                var currentUserId = this.GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var contacts = await _userService.GetUserContactsAsync(currentUserId.Value);
                return Ok(contacts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/contacts")]
        public async Task<IActionResult> AddUserContact(Guid id, [FromBody] AddContactRequest request)
        {
            try
            {
                var currentUserId = this.GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (currentUserId != id && !this.IsCurrentUserInRole(UserRole.Admin))
                    return Forbid("You can only modify your own contacts");

                var user = await _userService.AddUserContactAsync(id, request);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/contacts/{contactId}")]
        public async Task<IActionResult> UpdateUserContact(Guid id, Guid contactId, [FromBody] UpdateContactRequest request)
        {
            try
            {
                var currentUserId = this.GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (currentUserId != id && !this.IsCurrentUserInRole(UserRole.Admin))
                    return Forbid("You can only modify your own contacts");

                request.Id = contactId;
                var user = await _userService.UpdateUserContactAsync(id, request);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}/contacts/{contactId}")]
        public async Task<IActionResult> RemoveUserContact(Guid id, Guid contactId)
        {
            try
            {
                var currentUserId = this.GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (currentUserId != id && !this.IsCurrentUserInRole(UserRole.Admin))
                    return Forbid("You can only modify your own contacts");

                await _userService.RemoveUserContactAsync(id, contactId);
                return Ok(new { message = "Contact removed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }

}

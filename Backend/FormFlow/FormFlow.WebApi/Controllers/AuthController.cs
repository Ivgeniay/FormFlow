using Microsoft.AspNetCore.Authorization;
using FormFlow.WebApi.Common.Extensions;
using FormFlow.Application.DTOs.Users;
using FormFlow.Application.Interfaces;
using FormFlow.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.RegisterUserAsync(request);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new
            {
                user = result.User,
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                accessTokenExpiry = result.AccessTokenExpiry,
                refreshTokenExpiry = result.RefreshTokenExpiry,
                authType = result.AuthType.ToString()
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] EmailLoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.AuthenticateWithEmailAsync(request);

            if (!result.IsSuccess)
                return Unauthorized(new { message = result.ErrorMessage });

            return Ok(new
            {
                user = result.User,
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                accessTokenExpiry = result.AccessTokenExpiry,
                refreshTokenExpiry = result.RefreshTokenExpiry,
                authType = result.AuthType.ToString()
            });
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.AuthenticateWithGoogleAsync(request);

            if (!result.IsSuccess)
                return Unauthorized(new { message = result.ErrorMessage });

            return Ok(new
            {
                user = result.User,
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                accessTokenExpiry = result.AccessTokenExpiry,
                refreshTokenExpiry = result.RefreshTokenExpiry,
                authType = result.AuthType.ToString()
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.RefreshTokenAsync(request);

            if (!result.IsSuccess)
                return Unauthorized(new { message = result.ErrorMessage });

            return Ok(new
            {
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                accessTokenExpiry = result.AccessTokenExpiry,
                refreshTokenExpiry = result.RefreshTokenExpiry
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var userId = this.GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { message = "Invalid user context" });

            var result = await _userService.LogoutAsync(userId.Value, request);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ValidateTokenAsync(request);

            if (!result.IsValid)
                return Unauthorized(new { message = result.ErrorMessage });

            return Ok(new
            {
                isValid = result.IsValid,
                claims = result.Claims
            });
        }

        [HttpPut("change-role")]
        [Authorize]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var promotingUserId = this.GetCurrentUserId();
            if (promotingUserId == null)
                return Unauthorized(new { message = "Invalid user context" });

            var result = await _userService.ChangeUserRoleAsync(request, promotingUserId.Value);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new
            {
                user = result.User,
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                accessTokenExpiry = result.AccessTokenExpiry,
                refreshTokenExpiry = result.RefreshTokenExpiry,
                message = "Role changed successfully"
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = this.GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { message = "Invalid user context" });

            try
            {
                var user = await _userService.GetUserByIdAsync(userId.Value);
                return Ok(user);
            }
            catch
            {
                return NotFound(new { message = "User not found" });
            }
        }

        [HttpPost("check-email")]
        public async Task<IActionResult> CheckEmailExists([FromBody] CheckEmailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { message = "Email is required" });

            var exists = await _userService.EmailExistsAsync(request.Email);
            return Ok(new { exists });
        }

        [HttpPost("check-username")]
        public async Task<IActionResult> CheckUsernameExists([FromBody] CheckUsernameRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName))
                return BadRequest(new { message = "Username is required" });

            var exists = await _userService.UserNameExistsAsync(request.UserName);
            return Ok(new { exists });
        }
    }

    public class CheckEmailRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class CheckUsernameRequest
    {
        public string UserName { get; set; } = string.Empty;
    }
}

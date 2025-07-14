using FormFlow.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using FormFlow.WebApi.Common.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ApiTokenController : ControllerBase
    {
        private readonly IApiTokenService _apiTokenService;

        public ApiTokenController(IApiTokenService apiTokenService)
        {
            _apiTokenService = apiTokenService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateToken()
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var token = await _apiTokenService.GenerateTokenAsync(userId.Value);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentToken()
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var token = await _apiTokenService.GetActiveTokenAsync(userId.Value);
                
                if (token == null)
                    return NotFound(new { message = "No active API token found" });
                    
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetTokenHistory()
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var tokens = await _apiTokenService.GetUserTokensAsync(userId.Value);
                return Ok(tokens);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{tokenId}")]
        public async Task<IActionResult> RevokeToken(Guid tokenId)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                await _apiTokenService.RevokeTokenAsync(tokenId, userId.Value);
                return Ok(new { message = "Token revoked successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("revoke-all")]
        public async Task<IActionResult> RevokeAllTokens()
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                await _apiTokenService.RevokeAllUserTokensAsync(userId.Value);
                return Ok(new { message = "All tokens revoked successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
using FormFlow.Application.Interfaces;
using FormFlow.WebApi.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LikeController : ControllerBase
    {
        private readonly ILikeService _likeService;

        public LikeController(ILikeService likeService)
        {
            _likeService = likeService;
        }

        [HttpPost("toggle")]
        public IActionResult ToggleLike([FromBody] ToggleLikeRequest request)
        {
            return BadRequest(new
            {
                message = "Use SignalR WebSocket connection for real-time likes",
                signalrHub = "/hubs/template-activity",
                method = "ToggleLike"
            });
        }

        [HttpPost("add")]
        public IActionResult AddLike([FromBody] AddLikeRequest request)
        {
            return BadRequest(new
            {
                message = "Use SignalR WebSocket connection for real-time likes",
                signalrHub = "/hubs/template-activity",
                method = "ToggleLike"
            });
        }

        [HttpPost("remove")]
        public IActionResult RemoveLike([FromBody] RemoveLikeRequest request)
        {
            return BadRequest(new
            {
                message = "Use SignalR WebSocket connection for real-time likes",
                signalrHub = "/hubs/template-activity",
                method = "ToggleLike"
            });
        }

        [HttpGet("template/{templateId}")]
        public async Task<IActionResult> GetLikesByTemplate(
            Guid templateId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _likeService.GetLikesPagedAsync(templateId, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("template/{templateId}/count")]
        public async Task<IActionResult> GetLikesCount(Guid templateId)
        {
            try
            {
                var count = await _likeService.GetLikesCountAsync(templateId);
                return Ok(new { templateId, likesCount = count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("template/{templateId}/user-liked")]
        [Authorize]
        public async Task<IActionResult> HasUserLiked(Guid templateId)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var hasLiked = await _likeService.HasUserLikedAsync(userId.Value, templateId);
                return Ok(new { templateId, hasLiked });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/liked-templates")]
        [Authorize]
        public async Task<IActionResult> GetUserLikedTemplates(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var result = await _likeService.GetUserLikedTemplatesPagedAsync(userId.Value, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("template/{templateId}/stats")]
        public async Task<IActionResult> GetLikeStats(Guid templateId)
        {
            try
            {
                var likesCount = await _likeService.GetLikesCountAsync(templateId);

                var userLiked = false;
                var userId = this.GetCurrentUserId();
                if (userId.HasValue)
                {
                    userLiked = await _likeService.HasUserLikedAsync(userId.Value, templateId);
                }

                return Ok(new
                {
                    templateId,
                    likesCount,
                    userLiked,
                    isAuthenticated = userId.HasValue
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }


    public class ToggleLikeRequest
    {
        public Guid TemplateId { get; set; }
    }

    public class AddLikeRequest
    {
        public Guid TemplateId { get; set; }
    }

    public class RemoveLikeRequest
    {
        public Guid TemplateId { get; set; }
    }
}

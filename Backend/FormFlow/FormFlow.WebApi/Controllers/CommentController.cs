using Microsoft.AspNetCore.Authorization;
using FormFlow.Application.DTOs.Comments;
using FormFlow.WebApi.Common.Extensions;
using FormFlow.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public IActionResult AddComment([FromBody] AddCommentRequest request)
        {
            return BadRequest(new
            {
                message = "Use SignalR WebSocket connection for real-time comments",
                signalrHub = "/hubs/template-activity",
                method = "AddComment"
            });
        }

        [HttpGet("template/{templateId}")]
        public async Task<IActionResult> GetCommentsByTemplate(
            Guid templateId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _commentService.GetCommentsPagedAsync(templateId, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("template/{templateId}/recent")]
        public async Task<IActionResult> GetRecentComments(
            Guid templateId,
            [FromQuery] int count = 10)
        {
            try
            {
                var comments = await _commentService.GetRecentCommentsAsync(templateId, count);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{commentId}")]
        public async Task<IActionResult> GetComment(Guid commentId)
        {
            try
            {
                var comment = await _commentService.GetCommentByIdAsync(commentId);
                return Ok(comment);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            try
            {
                var userId = HttpContext.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (!await _commentService.CanDeleteCommentAsync(commentId, userId.Value))
                    return Forbid("You don't have permission to delete this comment");

                await _commentService.DeleteCommentAsync(commentId, userId.Value);

                return Ok(new { message = "Comment deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("template/{templateId}/count")]
        public async Task<IActionResult> GetCommentsCount(Guid templateId)
        {
            try
            {
                var count = await _commentService.GetCommentsCountAsync(templateId);
                return Ok(new { templateId, commentsCount = count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("template/{templateId}/can-comment")]
        [Authorize]
        public async Task<IActionResult> CanUserComment(Guid templateId)
        {
            try
            {
                var userId = HttpContext.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var canComment = await _commentService.CanUserCommentAsync(templateId, userId.Value);
                return Ok(new { templateId, canComment });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

}

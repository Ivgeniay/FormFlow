using FormFlow.Application.Interfaces;
using FormFlow.WebApi.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ITemplateService _templateService;

        public AnalyticsController(IAnalyticsService analyticsService, ITemplateService templateService)
        {
            _analyticsService = analyticsService;
            _templateService = templateService;
        }

        [HttpGet("template/{templateId}/basic")]
        public async Task<IActionResult> GetBasicStats(Guid templateId)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (!await _templateService.CanUserEditTemplateAsync(templateId, userId.Value))
                    return Forbid("Only template author and admins can view analytics");

                var result = await _analyticsService.GetBasicStatsAsync(templateId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("template/{templateId}/time")]
        public async Task<IActionResult> GetTimeAnalytics(Guid templateId, [FromBody] TimeAnalyticsQuery query)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (!await _templateService.CanUserEditTemplateAsync(templateId, userId.Value))
                    return Forbid("Only template author and admins can view analytics");

                var result = await _analyticsService.GetTimeAnalyticsAsync(templateId, query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

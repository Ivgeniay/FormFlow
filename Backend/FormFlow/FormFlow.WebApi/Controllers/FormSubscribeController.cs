using FormFlow.Application.Interfaces;
using FormFlow.Domain.Models.General;
using FormFlow.WebApi.Common.Attributes;
using FormFlow.WebApi.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FormSubscribeController : ControllerBase
    {
        private readonly IFormSubscribeService _formSubscribeService;
        private readonly ITemplateService _templateService;

        public FormSubscribeController(IFormSubscribeService formSubscribeService, ITemplateService templateService)
        {
            _formSubscribeService = formSubscribeService;
            _templateService = templateService;
        }

        [HttpPost("template/{templateId}")]
        public async Task<IActionResult> SubscribeToTemplate(Guid templateId)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var success = await _formSubscribeService.SubscribeAsync(userId.Value, templateId);

                if (success)
                    return Ok(new { message = "Successfully subscribed to template notifications", templateId, subscribed = true });
                else
                    return BadRequest(new { message = "Already subscribed to this template", templateId, subscribed = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("template/{templateId}")]
        public async Task<IActionResult> UnsubscribeFromTemplate(Guid templateId)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var success = await _formSubscribeService.UnsubscribeAsync(userId.Value, templateId);

                if (success)
                    return Ok(new { message = "Successfully unsubscribed from template notifications", templateId, subscribed = false });
                else
                    return NotFound(new { message = "No subscription found for this template", templateId, subscribed = false });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("template/{templateId}/status")]
        public async Task<IActionResult> GetSubscriptionStatus(Guid templateId)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var isSubscribed = await _formSubscribeService.IsSubscribedAsync(userId.Value, templateId);
                return Ok(new { templateId, subscribed = isSubscribed });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMySubscriptions()
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var subscriptions = await _formSubscribeService.GetUserSubscriptionsAsync(userId.Value);
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("template/{templateId}/subscribers")]
        public async Task<IActionResult> GetTemplateSubscribers(Guid templateId)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var isAdmin = this.IsCurrentUserInRole(UserRole.Admin);
                var isTemplateAuthor = await _templateService.IsUserTemplateAuthorAsync(templateId, userId.Value);

                if (!isAdmin && !isTemplateAuthor)
                    return Forbid("Only template author and admins can view subscribers");

                var subscribers = await _formSubscribeService.GetSubscribersAsync(templateId);
                return Ok(subscribers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("template/{templateId}/toggle")]
        public async Task<IActionResult> ToggleSubscription(Guid templateId)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var isCurrentlySubscribed = await _formSubscribeService.IsSubscribedAsync(userId.Value, templateId);

                if (isCurrentlySubscribed)
                {
                    var unsubscribeSuccess = await _formSubscribeService.UnsubscribeAsync(userId.Value, templateId);
                    return Ok(new
                    {
                        message = "Successfully unsubscribed from template notifications",
                        templateId,
                        subscribed = false,
                        action = "unsubscribed"
                    });
                }
                else
                {
                    var subscribeSuccess = await _formSubscribeService.SubscribeAsync(userId.Value, templateId);
                    return Ok(new
                    {
                        message = "Successfully subscribed to template notifications",
                        templateId,
                        subscribed = true,
                        action = "subscribed"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("stats")]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetSubscriptionStats()
        {
            try
            {
                return Ok(new
                {
                    message = "Subscription statistics endpoint - implementation needed",
                    note = "This endpoint can return total subscriptions, most subscribed templates, etc."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("template/{templateId}/has-subscribers")]
        public async Task<IActionResult> CheckTemplateHasSubscribers(Guid templateId)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var isAdmin = this.IsCurrentUserInRole(UserRole.Admin);
                var isTemplateAuthor = await _templateService.IsUserTemplateAuthorAsync(templateId, userId.Value);

                if (!isAdmin && !isTemplateAuthor)
                    return Forbid("Only template author and admins can check subscriber status");

                var hasSubscribers = await _formSubscribeService.GetSubscribersAsync(templateId);
                return Ok(new { templateId, hasSubscribers = hasSubscribers.Any() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

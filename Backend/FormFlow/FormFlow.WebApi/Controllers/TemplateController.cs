using FormFlow.Application.DTOs.Templates;
using FormFlow.Application.Interfaces;
using FormFlow.Domain.Interfaces.Services;
using FormFlow.Domain.Models.General;
using FormFlow.WebApi.Common.Attributes;
using FormFlow.WebApi.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateService _templateService;
        private readonly IAiTemplateService _aiTemplateService;

        public TemplateController(ITemplateService templateService, IAiTemplateService aiTemplateService)
        {
            _templateService = templateService;
            _aiTemplateService = aiTemplateService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateTemplateRequest request)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var template = await _templateService.CreateTemplateAsync(request, userId.Value);
                return Ok(template);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTemplate(Guid id)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                var template = await _templateService.GetTemplateByIdAsync(id, userId.HasValue ? userId.Value : null);
                return Ok(template);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTemplate(Guid id, [FromBody] UpdateTemplateRequest request)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var template = await _templateService.UpdateTemplateAsync(request, userId.Value);
                return Ok(template);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTemplate(Guid id)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                await _templateService.DeleteTemplateAsync(id, userId.Value);
                return Ok(new { message = "Template deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("undelete")]
        [Authorize]
        public async Task<IActionResult> UnDeleteTemplate([FromBody] UnDeleteTemplateRequest request)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                await _templateService.UnDeleteTemplateAsync(request.TemplateId, userId.Value);
                return Ok(new { message = "Template deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete-multiple")]
        [Authorize]
        public async Task<IActionResult> DeleteTemplates([FromBody] Guid[] templateGuids)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var result = await _templateService.DeleteTemplatesAsync(templateGuids, userId.Value);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/publish")]
        [Authorize]
        public async Task<IActionResult> PublishTemplate(Guid id)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var template = await _templateService.PublishTemplateAsync(id, userId.Value);
                return Ok(template);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("unarchive")]
        [Authorize]
        public async Task<IActionResult> UnarchiveTemplates([FromBody] Guid[] templateGuids)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var result = await _templateService.UnarchiveTemplatesAsync(templateGuids, userId.Value);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/unpublish")]
        [Authorize]
        public async Task<IActionResult> UnpublishTemplate(Guid id)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var template = await _templateService.ArchiveTemplateAsync(id, userId.Value);
                return Ok(template);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("archive")]
        [Authorize]
        public async Task<IActionResult> ArchiveTemplates([FromBody]Guid[] templateGuids)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user contex" });

                var result = await _templateService.ArchiveTemplatesAsync(templateGuids, userId.Value);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(new{ message = ex.Message });
            }
        }


        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularTemplates(
            [FromQuery] int count = 10,
            [FromQuery] int page = 1
            )
        {
            try
            {
                var templates = await _templateService.GetPopularTemplatesAsync(page, count);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestTemplates([FromQuery] int count = 10)
        {
            try
            {
                var templates = await _templateService.GetLatestTemplatesAsync(count);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("by-tag/{tagName}")]
        public async Task<IActionResult> GetTemplatesByTag(
            string tagName,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var templates = await _templateService.GetTemplatesByTagNameAsync(tagName, page, pageSize);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("admin")]
        [Authorize]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetAllTemplatesForAdmin(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _templateService.GetTemplatesPagedForAdminAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{baseTemplateId}/versions")]
        [Authorize]
        public async Task<IActionResult> GetTemplateVersionsForUser(Guid baseTemplateId, [FromQuery] Guid userId)
        {
            try
            {
                var curUserId = this.GetCurrentUserId();
                if (curUserId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var versions = await _templateService.GetAllVersionsForUserAsync(baseTemplateId, curUserId.Value, userId);
                return Ok(versions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{baseTemplateId}/allversions")]
        [Authorize]
        public async Task<IActionResult> GetAllTemplateVersions(Guid baseTemplateId)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var versions = await _templateService.GetAllVersionsAsync(baseTemplateId, userId.Value);
                return Ok(versions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{baseTemplateId}/version/{version}")]
        public async Task<IActionResult> GetSpecificVersion(Guid baseTemplateId, int version)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                var template = await _templateService.GetSpecificVersionAsync(baseTemplateId, version, userId);
                return Ok(template);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserTemplates(
            Guid userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var currentUserId = this.GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (currentUserId != userId && !this.IsCurrentUserInRole(UserRole.Admin))
                    return Forbid("Access denied");

                var result = await _templateService.GetTemplatesByAuthorPagedAsync(userId, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("accessible")]
        [Authorize]
        public async Task<IActionResult> GetAccessibleTemplates(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var result = await _templateService.GetUserAccessibleTemplatesPagedAsync(userId.Value, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/new-version")]
        [Authorize]
        public async Task<IActionResult> CreateNewVersion(Guid id, [FromBody] CreateNewVersionRequest request)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                request.BaseTemplateId = id;
                var template = await _templateService.CreateNewVersionAsync(request, request.AuthorId);
                return Ok(template);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("{id}/version-info")]
        [Authorize]
        public async Task<IActionResult> GetVersionInfo(Guid id)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var versionInfo = await _templateService.GetVersionInfoAsync(id, userId.Value);
                return Ok(versionInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/tags")]
        [Authorize]
        public async Task<IActionResult> UpdateTemplateTags(Guid id, [FromBody] UpdateTemplateTagsRequest request)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var template = await _templateService.UpdateTemplateTagsAsync(id, request, userId.Value);
                return Ok(template);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/allowed-users")]
        [Authorize]
        public async Task<IActionResult> UpdateTemplateAllowedUsers(Guid id, [FromBody] UpdateTemplateAllowedUsersRequest request)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var template = await _templateService.UpdateTemplateAllowedUsersAsync(id, request, userId.Value);
                return Ok(template);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("generate-ai")]
        [Authorize]
        public async Task<IActionResult> GenerateTemplateFromAI([FromBody] GenerateTemplateRequest request)
        {
            try
            {
                var result = await _aiTemplateService.GenerateFromPromptAsync(request.Promt);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class GenerateTemplateRequest
    {
        public string Promt { get; set; } = string.Empty;
    }
    public class UnDeleteTemplateRequest
    {
        public Guid TemplateId { get; set; }
    }
}

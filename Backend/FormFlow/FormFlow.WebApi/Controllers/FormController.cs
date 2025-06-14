using FormFlow.Application.DTOs.Forms;
using FormFlow.Application.Interfaces;
using FormFlow.Domain.Models.General;
using FormFlow.WebApi.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormController : ControllerBase
    {
        private readonly IFormService _formService;
        private readonly IFormSubscribeService _formSubscribeService;

        public FormController(IFormService formService, IFormSubscribeService formSubscribeService)
        {
            _formService = formService;
            _formSubscribeService = formSubscribeService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitForm([FromBody] SubmitFormRequest request)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var form = await _formService.SubmitFormAsync(request, userId.Value);

                await _formSubscribeService.NotifySubscribersAsync(form.Id);

                return Ok(form);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetForm(Guid id)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (!await _formService.CanUserViewFormAsync(id, userId.Value))
                    return Forbid("You don't have permission to view this form");

                var form = await _formService.GetFormByIdAsync(id, userId.Value);
                return Ok(form);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateForm(Guid formId, [FromBody] UpdateFormRequest request)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (!await _formService.CanUserEditFormAsync(formId, userId.Value))
                    return Forbid("You don't have permission to edit this form");

                request.Id = formId;
                var form = await _formService.UpdateFormAsync(request, userId.Value);
                return Ok(form);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteForm(Guid formId)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (!await _formService.CanUserEditFormAsync(formId, userId.Value))
                    return Forbid("You don't have permission to delete this form");

                await _formService.DeleteFormAsync(formId, userId.Value);
                return Ok(new { message = "Form deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("template/{templateId}")]
        [Authorize]
        public async Task<IActionResult> GetFormsByTemplate(
            Guid templateId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var result = await _formService.GetFormsByTemplatePagedAsync(templateId, page, pageSize, userId.Value);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetFormsByUser(
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
                    return Forbid("You can only view your own forms");

                var result = await _formService.GetFormsByUserPagedAsync(userId, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("template/{templateId}/user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserFormForTemplate(Guid templateId, Guid userId)
        {
            try
            {
                var currentUserId = this.GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (currentUserId != userId && !this.IsCurrentUserInRole(UserRole.Admin))
                    return Forbid("You can only view your own forms");

                var form = await _formService.GetUserFormForTemplateAsync(templateId, userId);

                if (form == null)
                    return NotFound(new { message = "Form not found" });

                return Ok(form);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("template/{templateId}/my")]
        [Authorize]
        public async Task<IActionResult> GetMyFormForTemplate(Guid templateId)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var form = await _formService.GetUserFormForTemplateAsync(templateId, userId.Value);

                if (form == null)
                    return NotFound(new { message = "You haven't submitted a form for this template" });

                return Ok(form);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("template/{templateId}/submitted")]
        [Authorize]
        public async Task<IActionResult> HasUserSubmittedForm(Guid templateId)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var hasSubmitted = await _formService.HasUserSubmittedFormAsync(templateId, userId.Value);
                return Ok(new { hasSubmitted, templateId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}/all-versions/{baseTemplateId}")]
        [Authorize]
        public async Task<IActionResult> GetUserFormsForAllVersions(Guid userId, Guid baseTemplateId)
        {
            try
            {
                var currentUserId = this.GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (currentUserId != userId && !this.IsCurrentUserInRole(UserRole.Admin))
                    return Forbid("You can only view your own forms");

                var forms = await _formService.GetUserFormsForAllVersionsAsync(baseTemplateId, userId);
                return Ok(forms);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyForms(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var result = await _formService.GetFormsByUserPagedAsync(userId.Value, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}

using FormFlow.Domain.Interfaces.Services;
using FormFlow.Domain.Models.General;
using FormFlow.WebApi.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        [Authorize]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> SendEmail([FromBody] EmailTemplate request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var success = await _emailService.SendEmailAsync(request);

                if (success)
                    return Ok(new { message = "Email sent successfully" });
                else
                    return BadRequest(new { message = "Failed to send email" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }

    public class SendFormAnswersRequest
    {
        public string ToEmail { get; set; } = string.Empty;
        public Guid FormId { get; set; }
    }

}

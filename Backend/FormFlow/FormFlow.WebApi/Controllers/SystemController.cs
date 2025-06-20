using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemController : ControllerBase
    {
        [HttpGet("ping")]
        public async Task<IActionResult> GetPing()
        {
            return Ok("Pong");
        }
    }
}

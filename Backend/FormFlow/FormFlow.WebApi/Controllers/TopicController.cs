using FormFlow.Application.Interfaces;
using FormFlow.Domain.Models.General;
using FormFlow.WebApi.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTopics(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (pageSize > 100) pageSize = 100;
                if (pageSize < 1) pageSize = 20;
                if (page < 1) page = 1;

                var result = await _topicService.GetTopicsAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetTopicsList([FromQuery] int count = 10)
        {
            try
            {
                if (count > 100) count = 100;
                if (count < 1) count = 10;

                var topics = await _topicService.GetTopicsAsync(count);
                return Ok(topics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopic(Guid id)
        {
            try
            {
                var topic = await _topicService.GetTopicByIdAsync(id);
                if (topic == null)
                    return NotFound(new { message = "Topic not found" });

                return Ok(topic);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> CreateTopic([FromBody] CreateTopicRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return BadRequest(new { message = "Topic name is required" });

                var topic = await _topicService.CreateTopicAsync(request.Name);
                return Ok(topic);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> UpdateTopic(Guid id, [FromBody] UpdateTopicRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return BadRequest(new { message = "Topic name is required" });

                var topic = await _topicService.UpdateTopicAsync(id, request.Name);
                return Ok(topic);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> DeleteTopic(Guid id)
        {
            try
            {
                await _topicService.DeleteTopicAsync(id);
                return Ok(new { message = "Topic deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/exists")]
        public async Task<IActionResult> TopicExists(Guid id)
        {
            try
            {
                var exists = await _topicService.TopicExistsAsync(id);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("check-name")]
        public async Task<IActionResult> CheckTopicName([FromBody] CheckTopicNameRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return BadRequest(new { message = "Topic name is required" });

                var exists = await _topicService.TopicNameExistsAsync(request.Name);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class CreateTopicRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateTopicRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class CheckTopicNameRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}

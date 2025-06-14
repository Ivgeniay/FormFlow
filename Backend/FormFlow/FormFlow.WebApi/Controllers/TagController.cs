using FormFlow.Application.Interfaces;
using FormFlow.Domain.Models.General;
using FormFlow.WebApi.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTags(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var result = await _tagService.GetTagsPagedAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTags(
            [FromQuery] string q,
            [FromQuery] int limit = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                    return BadRequest(new { message = "Query must be at least 2 characters long" });

                var tags = await _tagService.SearchTagsAsync(q, limit);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("cloud")]
        public async Task<IActionResult> GetTagCloud([FromQuery] int maxTags = 50)
        {
            try
            {
                var tagCloud = await _tagService.GetTagCloudAsync(maxTags);
                return Ok(tagCloud);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularTags([FromQuery] int count = 20)
        {
            try
            {
                var tags = await _tagService.GetMostPopularTagsAsync(count);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{tagName}/templates")]
        public async Task<IActionResult> GetTemplatesByTag(
            string tagName,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var templates = await _tagService.GetTemplatesByTagNameAsync(tagName, page, pageSize);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTag(Guid id)
        {
            try
            {
                var tag = await _tagService.GetTagByIdAsync(id);
                return Ok(tag);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("by-name/{tagName}")]
        public async Task<IActionResult> GetTagByName(string tagName)
        {
            try
            {
                var tag = await _tagService.GetTagByNameAsync(tagName);
                return Ok(tag);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return BadRequest(new { message = "Tag name is required" });

                if (await _tagService.TagExistsAsync(request.Name))
                    return BadRequest(new { message = "Tag already exists" });

                var tag = await _tagService.CreateTagAsync(request.Name);
                return Ok(tag);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("cleanup")]
        [Authorize]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> CleanupUnusedTags()
        {
            try
            {
                await _tagService.CleanupUnusedTagsAsync();
                return Ok(new { message = "Unused tags cleaned up successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/recalculate-usage")]
        [Authorize]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> RecalculateTagUsage(Guid id)
        {
            try
            {
                await _tagService.RecalculateTagUsageAsync(id);
                var tag = await _tagService.GetTagByIdAsync(id);
                return Ok(new { message = "Tag usage recalculated successfully", tag });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("stats")]
        [Authorize]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> GetTagStats()
        {
            try
            {
                var allTags = await _tagService.GetTagsPagedAsync(1, int.MaxValue);
                var popularTags = await _tagService.GetMostPopularTagsAsync(10);

                var stats = new
                {
                    TotalTags = allTags.Pagination.TotalCount,
                    UsedTags = allTags.Data.Count(t => t.UsageCount > 0),
                    UnusedTags = allTags.Data.Count(t => t.UsageCount == 0),
                    TopTags = popularTags.Take(5).Select(t => new { t.Name, t.UsageCount }),
                    AverageUsage = allTags.Data.Any() ? allTags.Data.Average(t => t.UsageCount) : 0
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("autocomplete")]
        public async Task<IActionResult> GetTagsForAutocomplete(
            [FromQuery] string q,
            [FromQuery] int limit = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                    return Ok(new List<string>());

                var tags = await _tagService.SearchTagsAsync(q, limit);
                var tagNames = tags.Select(t => t.Name).ToList();
                return Ok(tagNames);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class CreateTagRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}

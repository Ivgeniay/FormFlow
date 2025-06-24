using FormFlow.Domain.Interfaces.Services;
using FormFlow.Domain.Models.General;
using FormFlow.Domain.Models.SearchService;
using FormFlow.WebApi.Common.Attributes;
using FormFlow.WebApi.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("templates")]
        public async Task<IActionResult> SearchTemplates(
            [FromQuery] string q = "",
            [FromQuery] string[]? tags = null,
            [FromQuery] string? author = null,
            [FromQuery] string? topic = null,
            [FromQuery] int sortBy = 0,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (pageSize > 100) pageSize = 100;
                if (pageSize < 1) pageSize = 20;
                if (page < 1) page = 1;

                var sortByEnum = (SearchSortBy)sortBy;

                var searchQuery = new SearchQuery
                {
                    Query = q?.Trim() ?? "",
                    Topic = topic?.Trim(),
                    Tags = tags?.Where(t => !string.IsNullOrWhiteSpace(t)).ToList(),
                    AuthorName = author?.Trim(),
                    SortBy = sortByEnum,
                    Page = page,
                    PageSize = pageSize,
                    IncludeDeleted = false,
                    IncludeArchived = false,
                    IncludeUnpublished = false
                };

                var result = await _searchService.SearchTemplatesAsync(searchQuery);

                return Ok(new
                {
                    templates = result.Results,
                    pagination = new
                    {
                        currentPage = page,
                        pageSize = pageSize,
                        totalCount = result.TotalCount,
                        totalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize),
                        hasNext = page * pageSize < result.TotalCount,
                        hasPrevious = page > 1
                    },
                    searchInfo = new
                    {
                        query = searchQuery.Query,
                        topic = searchQuery.Topic,
                        tags = searchQuery.Tags ?? new List<string>(),
                        author = searchQuery.AuthorName,
                        sortBy = sortBy.ToString(),
                        searchTime = result.SearchTime.TotalMilliseconds
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSearchSuggestions(
            [FromQuery] string q,
            [FromQuery] int limit = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                    return Ok(new List<string>());

                if (limit > 50) limit = 50;
                if (limit < 1) limit = 10;

                var suggestions = await _searchService.GetSearchSuggestionsAsync(q, limit);
                return Ok(suggestions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("advanced")]
        public async Task<IActionResult> AdvancedSearch([FromBody] AdvancedSearchRequest request)
        {
            try
            {
                if (request.PageSize > 100) request.PageSize = 100;
                if (request.PageSize < 1) request.PageSize = 20;
                if (request.Page < 1) request.Page = 1;

                var searchQuery = new SearchQuery
                {
                    Query = request.Query?.Trim() ?? "",
                    Tags = request.Tags?.Where(t => !string.IsNullOrWhiteSpace(t)).ToList(),
                    AuthorName = request.AuthorName?.Trim(),
                    Topic = request.Topic?.Trim(),
                    SortBy = request.SortBy,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    CreatedAfter = request.CreatedAfter,
                    CreatedBefore = request.CreatedBefore,
                    IncludeDeleted = false,
                    IncludeArchived = request.IncludeArchived,
                    IncludeUnpublished = false
                };

                var result = await _searchService.SearchTemplatesAsync(searchQuery);

                return Ok(new
                {
                    templates = result.Results,
                    pagination = new
                    {
                        currentPage = request.Page,
                        pageSize = request.PageSize,
                        totalCount = result.TotalCount,
                        totalPages = (int)Math.Ceiling((double)result.TotalCount / request.PageSize),
                        hasNext = request.Page * request.PageSize < result.TotalCount,
                        hasPrevious = request.Page > 1
                    },
                    searchInfo = new
                    {
                        query = searchQuery.Query,
                        tags = searchQuery.Tags ?? new List<string>(),
                        author = searchQuery.AuthorName,
                        topic = searchQuery.Topic,
                        sortBy = request.SortBy.ToString(),
                        dateRange = new { from = request.CreatedAfter, to = request.CreatedBefore },
                        includeArchived = request.IncludeArchived,
                        searchTime = result.SearchTime.TotalMilliseconds
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("admin")]
        [Authorize]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> AdminSearch(
            [FromQuery] string q = "",
            [FromQuery] string[]? tags = null,
            [FromQuery] string? author = null,
            [FromQuery] string? topic = null,
            [FromQuery] SearchSortBy sortBy = SearchSortBy.Relevance,
            [FromQuery] bool includeDeleted = false,
            [FromQuery] bool includeArchived = true,
            [FromQuery] bool includeUnpublished = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (pageSize > 100) pageSize = 100;

                var searchQuery = new SearchQuery
                {
                    Query = q?.Trim() ?? "",
                    Tags = tags?.Where(t => !string.IsNullOrWhiteSpace(t)).ToList(),
                    AuthorName = author?.Trim(),
                    Topic = topic?.Trim(),
                    SortBy = sortBy,
                    Page = page,
                    PageSize = pageSize,
                    IncludeDeleted = includeDeleted,
                    IncludeArchived = includeArchived,
                    IncludeUnpublished = includeUnpublished
                };

                var result = await _searchService.SearchTemplatesAsync(searchQuery);

                return Ok(new
                {
                    templates = result.Results,
                    pagination = new
                    {
                        currentPage = page,
                        pageSize = pageSize,
                        totalCount = result.TotalCount,
                        totalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize),
                        hasNext = page * pageSize < result.TotalCount,
                        hasPrevious = page > 1
                    },
                    searchInfo = new
                    {
                        query = searchQuery.Query,
                        filters = new
                        {
                            tags = searchQuery.Tags ?? new List<string>(),
                            author = searchQuery.AuthorName,
                            topic = searchQuery.Topic,
                            includeDeleted,
                            includeArchived,
                            includeUnpublished
                        },
                        sortBy = sortBy.ToString(),
                        searchTime = result.SearchTime.TotalMilliseconds
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("reindex")]
        [Authorize]
        [RequireRole(UserRole.Admin)]
        public async Task<IActionResult> ReindexTemplates()
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                await _searchService.ReindexAllTemplatesAsync();

                return Ok(new
                {
                    message = "Template reindexing started successfully",
                    note = "This operation may take several minutes to complete",
                    startedAt = DateTime.UtcNow,
                    startedBy = userId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("quick")]
        public async Task<IActionResult> QuickSearch(
            [FromQuery] string q,
            [FromQuery] int limit = 5)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                    return Ok(new List<object>());

                if (limit > 20) limit = 20;

                var searchQuery = new SearchQuery
                {
                    Query = q.Trim(),
                    Page = 1,
                    PageSize = limit,
                    SortBy = SearchSortBy.Relevance,
                    IncludeDeleted = false,
                    IncludeArchived = false,
                    IncludeUnpublished = false
                };

                var result = await _searchService.SearchTemplatesAsync(searchQuery);

                var quickResults = result.Results.Select(template => new
                {
                    id = template.Id,
                    title = template.Title,
                    authorName = template.AuthorName,
                    tags = template.Tags.Take(3).ToList()
                }).ToList();

                return Ok(quickResults);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class AdvancedSearchRequest
    {
        public string Query { get; set; } = "";
        public List<string>? Tags { get; set; }
        public string? AuthorName { get; set; }
        public string? Topic { get; set; }
        public SearchSortBy SortBy { get; set; } = SearchSortBy.Relevance;
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public bool IncludeArchived { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}

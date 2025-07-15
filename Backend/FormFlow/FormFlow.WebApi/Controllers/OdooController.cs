using FormFlow.Application.DTOs.Templates;
using FormFlow.Application.Interfaces;
using FormFlow.Domain.Interfaces.Services;
using FormFlow.Domain.Models.General;
using FormFlow.Domain.Models.General.QuestionDetailsModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OdooController : ControllerBase
    {
        private readonly IApiTokenService _apiTokenService;
        private readonly ITemplateService _templateService;
        private readonly IFormService _formService;

        public OdooController(
            IApiTokenService apiTokenService,
            ITemplateService templateService,
            IFormService formService)
        {
            _apiTokenService = apiTokenService;
            _templateService = templateService;
            _formService = formService;
        }

        [HttpGet("templates")]
        public async Task<IActionResult> GetTemplates([FromHeader(Name = "X-API-Token")] string apiToken)
        {
            try
            {
                if (string.IsNullOrEmpty(apiToken))
                    return Unauthorized(new { message = "API token is required" });

                var userId = await _apiTokenService.GetUserIdByTokenAsync(apiToken);
                if (userId == null)
                    return Unauthorized(new { message = "Invalid API token" });

                await _apiTokenService.UpdateTokenLastUsedAsync(apiToken);

                PagedResult<TemplateDto> templates = await _templateService.GetTemplatesByAuthorPagedAsync(userId.Value, 1, 1000);
                
                var result = new List<object>();

            foreach (var t in templates.Data)
            {
                var totalResponses = await _formService.GetFormsByTemplatePagedAsync(t.Id, 1, 1, userId.Value);
                var questions = new List<object>();
                foreach (var q in t.Questions)
                {
                    try
                    {
                        var questionDetails = JsonConvert.DeserializeObject<QuestionDetails>(q.Data);
                            if (questionDetails != null)
                            {
                            
                            questions.Add(new
                            {
                                id = q.Id,
                                title = questionDetails?.Title ?? "",
                                description = questionDetails?.Description ?? "",
                                type = questionDetails?.Type,
                                order = q.Order,
                                isRequired = q.IsRequired
                            });
                        }
                    }
                    catch
                    {
                        questions.Add(new
                        {
                            id = q.Id,
                            title = "Invalid question data",
                            description = "",
                            type = "Unknown",
                            order = q.Order,
                            isRequired = q.IsRequired
                        });
                    }
                }

                result.Add(new
                {
                    id = t.Id,
                    title = t.Title,
                    description = t.Description,
                    author = t.AuthorName,
                    createdAt = t.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fff"),
                    isPublished = t.IsPublished,
                    totalResponses = totalResponses.Pagination.TotalCount,
                    questions = questions
                });
            }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("templates/{templateId}/aggregated")]
        public async Task<IActionResult> GetTemplateAggregatedData(
            Guid templateId, 
            [FromHeader(Name = "X-API-Token")] string apiToken)
        {
            try
            {
                if (string.IsNullOrEmpty(apiToken))
                    return Unauthorized(new { message = "API token is required" });

                var userId = await _apiTokenService.GetUserIdByTokenAsync(apiToken);
                if (userId == null)
                    return Unauthorized(new { message = "Invalid API token" });

                await _apiTokenService.UpdateTokenLastUsedAsync(apiToken);

                var template = await _templateService.GetTemplateByIdAsync(templateId, userId);
                if (template == null || template.AuthorId != userId.Value)
                    return NotFound(new { message = "Template not found or access denied" });

                var forms = await _formService.GetFormsByTemplatePagedAsync(templateId, 1, int.MaxValue, userId.Value);

                var result = new
                {
                    templateId = template.Id,
                    templateTitle = template.Title,
                    author = template.AuthorName,
                    totalResponses = forms.Pagination.TotalCount,
                    questions = new List<object>()
                };

                var questionAggregations = new List<object>();

                foreach (var question in template.Questions)
                {
                    try
                    {
                        var questionDetails = JsonConvert.DeserializeObject<QuestionDetails>(question.Data);
                        var aggregation = GetQuestionAggregation(question.Id, questionDetails?.Type.ToString() ?? "Unknown", forms.Data);
                        
                        questionAggregations.Add(new
                        {
                            questionId = question.Id,
                            title = questionDetails?.Title ?? "Unknown Question",
                            type = questionDetails?.Type.ToString() ?? "Unknown",
                            aggregatedResults = aggregation
                        });
                    }
                    catch
                    {
                        questionAggregations.Add(new
                        {
                            questionId = question.Id,
                            title = "Invalid question data",
                            type = "Unknown",
                            aggregatedResults = new { totalAnswers = 0 }
                        });
                    }
                }

                result = new
                {
                    templateId = template.Id,
                    templateTitle = template.Title,
                    author = template.AuthorName,
                    totalResponses = forms.Pagination.TotalCount,
                    questions = questionAggregations
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private object GetQuestionAggregation(Guid questionId, string questionType, object aggregatedData)
        {
            return questionType.ToLower() switch
            {
                "integer" => new { average = 0, min = 0, max = 0, totalAnswers = 0 },
                "shorttext" or "longtext" => new { mostPopularAnswers = new string[0], totalAnswers = 0 },
                "singlechoice" or "multiplechoice" => new { optionCounts = new object[0], totalAnswers = 0 },
                _ => new { totalAnswers = 0 }
            };
        }
    }
}
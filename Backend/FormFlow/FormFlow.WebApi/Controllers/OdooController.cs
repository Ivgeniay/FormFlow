using FormFlow.Application.DTOs.Forms;
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
                var questionAggregations = new List<object>();

                foreach (var question in template.Questions)
                {
                    try
                    {
                        var questionDetails = JsonConvert.DeserializeObject<QuestionDetails>(question.Data);
                        var aggregation = GetQuestionAggregation(question.Id, questionDetails?.Type.ToString() ?? "Unknown", forms);

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

                var result = new
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

        private object GetQuestionAggregation(Guid questionId, string questionType, PagedResult<FormDto> forms)
        {
            var answers = new List<object>();

            foreach (var form in forms.Data)
            {
                var question = form.Questions.FirstOrDefault(q => q.QuestionId == questionId);
                if (question?.Answer != null)
                {
                    answers.Add(question.Answer);
                }
            }

            return questionType.ToLower() switch
            {
                "scale" or "rating" => CalculateNumericAggregation(answers),
                "shorttext" or "longtext" => CalculateTextAggregation(answers),
                "singlechoice" or "dropdown" => CalculateSingleChoiceAggregation(answers),
                "multiplechoice" => CalculateMultipleChoiceAggregation(answers),
                "date" => CalculateDateAggregation(answers),
                "time" => CalculateTimeAggregation(answers),
                _ => new { totalAnswers = answers.Count }
            };
        }

        private object CalculateNumericAggregation(List<object> answers)
        {
            var values = new List<int>();
            
            foreach (var answer in answers)
            {
                if (answer != null && int.TryParse(answer?.ToString() ?? "", out int value))
                    values.Add(value);
            }

            if (values.Count == 0)
                return new { average = 0, min = 0, max = 0, totalAnswers = 0 };

            return new
            {
                average = Math.Round(values.Average(), 2),
                min = values.Min(),
                max = values.Max(),
                totalAnswers = values.Count
            };
        }

        private object CalculateTextAggregation(List<object> answers)
        {
            var texts = new List<string>();
            
            foreach (var answer in answers)
            {
                if (answer != null && !string.IsNullOrWhiteSpace(answer?.ToString() ?? ""))
                    texts.Add(answer?.ToString() ?? "");
            }

            var groups = texts
                .GroupBy(t => t)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => new { answer = g.Key, count = g.Count() })
                .ToArray();

            return new
            {
                mostPopularAnswers = groups,
                totalAnswers = texts.Count
            };
        }

        private object CalculateSingleChoiceAggregation(List<object> answers)
        {
            var choices = new List<string>();
            
            foreach (var answer in answers)
            {
                if (answer != null && !string.IsNullOrWhiteSpace(answer?.ToString() ?? ""))
                    choices.Add(answer?.ToString() ?? "");
            }

            var groups = choices
                .GroupBy(c => c)
                .OrderByDescending(g => g.Count())
                .Select(g => new { option = g.Key, count = g.Count() })
                .ToArray();

            return new
            {
                optionCounts = groups,
                totalAnswers = choices.Count
            };
        }

        private object CalculateMultipleChoiceAggregation(List<object> answers)
        {
            var allOptions = new List<string>();
            
            foreach (var answer in answers)
            {
                if (answer != null)
                {
                    try
                    {
                        var json = answer?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(json) && json.StartsWith("["))
                        {
                            var options = JsonConvert.DeserializeObject<string[]>(json);
                            if (options != null)
                                allOptions.AddRange(options);
                        }
                    }
                    catch { }
                }
            }

            var groups = allOptions
                .GroupBy(o => o)
                .OrderByDescending(g => g.Count())
                .Select(g => new { option = g.Key, count = g.Count() })
                .ToArray();

            return new
            {
                optionCounts = groups,
                totalAnswers = answers.Count(a => a != null)
            };
        }

        private object CalculateDateAggregation(List<object> answers)
        {
            var dates = new List<DateTime>();
            
            foreach (var answer in answers)
            {
                var dateStr = answer?.ToString() ?? "";
                if (DateTime.TryParse(dateStr, out DateTime date))
                    dates.Add(date);
            }

            if (dates.Count == 0)
                return new { earliestDate = "", latestDate = "", totalAnswers = 0 };

            return new
            {
                earliestDate = dates.Min().ToString("yyyy-MM-dd"),
                latestDate = dates.Max().ToString("yyyy-MM-dd"),
                totalAnswers = dates.Count
            };
        }

        private object CalculateTimeAggregation(List<object> answers)
        {
            var times = new List<string>();
            
            foreach (var answer in answers)
            {
                if (answer != null && !string.IsNullOrWhiteSpace(answer?.ToString() ?? ""))
                    times.Add(answer?.ToString() ?? "");
            }

            var groups = times
                .GroupBy(t => t)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => new { time = g.Key, count = g.Count() })
                .ToArray();

            return new
            {
                mostPopularTimes = groups,
                totalAnswers = times.Count
            };
        }

    }
}
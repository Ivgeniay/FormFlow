using Microsoft.AspNetCore.Authorization;
using FormFlow.WebApi.Common.Attributes;
using FormFlow.Application.Interfaces;
using FormFlow.Domain.Models.General;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [RequireRole(UserRole.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IColorThemeService _colorThemeService;
        private readonly ILanguageService _languageService;

        public AdminController(IColorThemeService colorThemeService, ILanguageService languageService)
        {
            _colorThemeService = colorThemeService;
            _languageService = languageService;
        }

        [HttpGet("color-themes")]
        public async Task<IActionResult> GetColorThemes()
        {
            try
            {
                var colorThemes = await _colorThemeService.GetAllAsync();
                return Ok(colorThemes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("color-themes/{id}")]
        public async Task<IActionResult> GetColorTheme(Guid id)
        {
            try
            {
                var colorTheme = await _colorThemeService.GetByIdAsync(id);
                if (colorTheme == null)
                    return NotFound(new { message = "Color theme not found" });

                return Ok(colorTheme);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("color-themes")]
        public async Task<IActionResult> CreateColorTheme([FromBody] ColorTheme colorTheme)
        {
            try
            {
                var created = await _colorThemeService.CreateAsync(colorTheme);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("color-themes/{id}")]
        public async Task<IActionResult> UpdateColorTheme(Guid id, [FromBody] ColorTheme colorTheme)
        {
            try
            {
                var updated = await _colorThemeService.UpdateAsync(id, colorTheme);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("color-themes/{id}")]
        public async Task<IActionResult> DeleteColorTheme(Guid id)
        {
            try
            {
                await _colorThemeService.DeleteAsync(id);
                return Ok(new { message = "Color theme deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("color-themes/{id}/set-default")]
        public async Task<IActionResult> SetDefaultColorTheme(Guid id)
        {
            try
            {
                var colorTheme = await _colorThemeService.SetDefaultAsync(id);
                return Ok(colorTheme);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("languages")]
        public async Task<IActionResult> GetLanguages()
        {
            try
            {
                var languages = await _languageService.GetAllAsync();
                return Ok(languages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("languages/{id}")]
        public async Task<IActionResult> GetLanguage(Guid id)
        {
            try
            {
                var language = await _languageService.GetByIdAsync(id);
                if (language == null)
                    return NotFound(new { message = "Language not found" });

                return Ok(language);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("languages")]
        public async Task<IActionResult> CreateLanguage([FromBody] Language language)
        {
            try
            {
                var created = await _languageService.CreateAsync(language);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("languages/{id}")]
        public async Task<IActionResult> UpdateLanguage(Guid id, [FromBody] Language language)
        {
            try
            {
                var updated = await _languageService.UpdateAsync(id, language);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("languages/{id}")]
        public async Task<IActionResult> DeleteLanguage(Guid id)
        {
            try
            {
                await _languageService.DeleteAsync(id);
                return Ok(new { message = "Language deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("languages/{id}/set-default")]
        public async Task<IActionResult> SetDefaultLanguage(Guid id)
        {
            try
            {
                var language = await _languageService.SetDefaultAsync(id);
                return Ok(language);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("color-themes/active")]
        public async Task<IActionResult> GetActiveColorThemes()
        {
            try
            {
                var colorThemes = await _colorThemeService.GetActiveAsync();
                return Ok(colorThemes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("languages/active")]
        public async Task<IActionResult> GetActiveLanguages()
        {
            try
            {
                var languages = await _languageService.GetActiveAsync();
                return Ok(languages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

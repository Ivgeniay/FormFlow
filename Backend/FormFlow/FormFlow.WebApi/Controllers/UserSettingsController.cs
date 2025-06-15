using FormFlow.Application.Interfaces;
using FormFlow.WebApi.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserSettingsController : ControllerBase
    {
        private readonly IUserSettingsService _userSettingsService;
        private readonly IColorThemeService _colorThemeService;
        private readonly ILanguageService _languageService;

        public UserSettingsController(
            IUserSettingsService userSettingsService,
            IColorThemeService colorThemeService,
            ILanguageService languageService)
        {
            _userSettingsService = userSettingsService;
            _colorThemeService = colorThemeService;
            _languageService = languageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMySettings()
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var settings = await _userSettingsService.GetByUserIdAsync(userId.Value);
                if (settings == null)
                {
                    settings = await _userSettingsService.CreateDefaultForUserAsync(userId.Value);
                }

                return Ok(settings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("color-theme/{colorThemeId}")]
        public async Task<IActionResult> SetColorTheme(Guid colorThemeId)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var settings = await _userSettingsService.SetColorThemeAsync(userId.Value, colorThemeId);
                return Ok(settings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("language/{languageId}")]
        public async Task<IActionResult> SetLanguage(Guid languageId)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var settings = await _userSettingsService.SetLanguageAsync(userId.Value, languageId);
                return Ok(settings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("color-themes")]
        public async Task<IActionResult> GetAvailableColorThemes()
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

        [HttpGet("languages")]
        public async Task<IActionResult> GetAvailableLanguages()
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

        [HttpGet("defaults")]
        public async Task<IActionResult> GetDefaults()
        {
            try
            {
                var defaultColorTheme = await _colorThemeService.GetDefaultAsync();
                var defaultLanguage = await _languageService.GetDefaultAsync();

                return Ok(new
                {
                    defaultColorTheme,
                    defaultLanguage
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetToDefaults()
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                var defaultColorTheme = await _colorThemeService.GetDefaultAsync();
                var defaultLanguage = await _languageService.GetDefaultAsync();

                if (defaultColorTheme == null || defaultLanguage == null)
                    return BadRequest(new { message = "Default settings not configured" });

                await _userSettingsService.DeleteByUserIdAsync(userId.Value);
                var newSettings = await _userSettingsService.CreateDefaultForUserAsync(userId.Value);

                return Ok(newSettings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("initialize")]
        public async Task<IActionResult> InitializeSettings()
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (await _userSettingsService.ExistsByUserIdAsync(userId.Value))
                    return BadRequest(new { message = "Settings already exist" });

                var settings = await _userSettingsService.CreateDefaultForUserAsync(userId.Value);
                return Ok(settings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

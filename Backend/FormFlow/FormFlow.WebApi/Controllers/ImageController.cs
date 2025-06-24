using FormFlow.Application.Interfaces;
using FormFlow.Domain.Interfaces.Services;
using FormFlow.WebApi.Common;
using FormFlow.WebApi.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ITemplateService _templateService;
        private readonly IImageStorageService _imageStorageService;

        public ImageController(ITemplateService templateService, IImageStorageService imageStorageService)
        {
            _templateService = templateService;
            _imageStorageService = imageStorageService;
        }

        [HttpPost("template/{templateId}/image")]
        [Authorize]
        public async Task<IActionResult> UploadTemplateImage(Guid templateId, IFormFile file)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "No file provided" });

                if (!IsValidImageFile(file))
                    return BadRequest(new { message = "Invalid file type. Only images are allowed" });

                if (!await _templateService.CanUserEditTemplateAsync(templateId, userId.Value))
                    return Forbid("You don't have permission to edit this template");

                var imageUrl = await _imageStorageService.UploadImageAsync(file.OpenReadStream(), file.FileName);

                var template = await _templateService.GetTemplateByIdAsync(templateId, userId);

                if (!string.IsNullOrEmpty(template.ImageUrl))
                {
                    await _imageStorageService.DeleteImageAsync(template.ImageUrl);
                }

                var success = await _templateService.UpdateTemplateImageAsync(templateId, imageUrl, userId.Value);
                if (!success)
                    return BadRequest(new { message = "Failed to update template image" });

                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpDelete("template/{templateId}/image")]
        [Authorize]
        public async Task<IActionResult> DeleteTemplateImage(Guid templateId)
        {
            try
            {
                var userId = this.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid user context" });

                if (!await _templateService.CanUserEditTemplateAsync(templateId, userId.Value))
                    return Forbid("You don't have permission to edit this template");

                var template = await _templateService.GetTemplateByIdAsync(templateId, userId);

                if (!string.IsNullOrEmpty(template.ImageUrl))
                {
                    await _imageStorageService.DeleteImageAsync(template.ImageUrl);
                }

                await _templateService.RemoveTemplateImageAsync(templateId, userId.Value);

                return Ok(new { message = "Image deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("template/{templateId}/image/exists")]
        public async Task<IActionResult> CheckTemplateImageExists(Guid templateId)
        {
            try
            {
                var template = await _templateService.GetTemplateByIdAsync(templateId);

                if (string.IsNullOrEmpty(template.ImageUrl))
                    return Ok(new { exists = false });

                var exists = await _imageStorageService.ImageExistsAsync(template.ImageUrl);
                return Ok(new { exists, imageUrl = template.ImageUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("template/{templateId}/image")]
        public async Task<IActionResult> GetTemplateImage(Guid templateId)
        {
            try
            {
                var template = await _templateService.GetTemplateByIdAsync(templateId);

                if (string.IsNullOrEmpty(template.ImageUrl))
                    return NoContent();

                var exists = await _imageStorageService.ImageExistsAsync(template.ImageUrl);
                if (exists)
                    return Ok(new { imageUrl = template.ImageUrl });
                else return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("proxy")]
        public async Task<IActionResult> ProxyImage([FromQuery] string imageUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl))
                    return BadRequest(new { message = "Image URL is required" });

                if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
                    return BadRequest(new { message = "Invalid image URL format" });

                var imageStream = await _imageStorageService.GetImageStreamAsync(imageUrl);

                if (imageStream == null)
                    return NotFound(new { message = "Image not found" });

                var contentType = GetContentTypeFromUrl(imageUrl);

                return File(imageStream, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving image", error = ex.Message });
            }
        }

        private static string GetContentTypeFromUrl(string imageUrl)
        {
            var extension = Path.GetExtension(imageUrl).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "image/jpeg"
            };
        }

        private bool IsValidImageFile(IFormFile file) =>
            Constants.IMAGE_ALLOWED_CONTENT_TYPES.Contains(file.ContentType);

    }
}

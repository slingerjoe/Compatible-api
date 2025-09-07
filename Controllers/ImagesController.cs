using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CompatibleAPI.Application.Interfaces;
using CompatibleAPI.Domain.Entities;
using CompatibleAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CompatibleAPI.Controllers
{
    /// <summary>
    /// Controller for handling image upload and management operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(
            IImageService imageService,
            ApplicationDbContext context,
            ILogger<ImagesController> logger)
        {
            _imageService = imageService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Uploads an image for a profile
        /// </summary>
        /// <param name="profileId">The ID of the profile</param>
        /// <param name="file">The image file to upload</param>
        /// <param name="isMain">Whether this should be the main photo</param>
        [HttpPost("upload/{profileId}")]
        [ProducesResponseType(typeof(Photo), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Photo>> UploadImage(Guid profileId, IFormFile file, [FromQuery] bool isMain = false)
        {
            try
            {
                // Validate profile exists
                var profile = await _context.Profiles.FindAsync(profileId);
                if (profile == null)
                {
                    return NotFound(new { error = "Profile not found" });
                }

                // Validate file
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No file provided" });
                }

                if (!_imageService.IsValidImage(file))
                {
                    return BadRequest(new { error = "Invalid image file. Supported formats: JPG, PNG, GIF, WebP. Max size: 10MB" });
                }

                // Upload to S3
                var imageUrl = await _imageService.UploadImageAsync(file, profileId);

                // Create photo record in database
                var photo = new Photo
                {
                    Id = Guid.NewGuid(),
                    Url = imageUrl,
                    ProfileId = profileId,
                    IsMain = isMain,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // If this is the main photo, unset other main photos
                if (isMain)
                {
                    var existingMainPhotos = await _context.Photos
                        .Where(p => p.ProfileId == profileId && p.IsMain)
                        .ToListAsync();

                    foreach (var existingPhoto in existingMainPhotos)
                    {
                        existingPhoto.IsMain = false;
                        existingPhoto.UpdatedAt = DateTime.UtcNow;
                    }
                }

                _context.Photos.Add(photo);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Image uploaded successfully for profile {ProfileId}: {ImageUrl}", profileId, imageUrl);

                return CreatedAtAction(nameof(GetPhoto), new { id = photo.Id }, photo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image for profile {ProfileId}", profileId);
                return StatusCode(500, new { error = "Failed to upload image", message = ex.Message });
            }
        }

        /// <summary>
        /// Gets a specific photo by ID
        /// </summary>
        /// <param name="id">The ID of the photo</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Photo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Photo>> GetPhoto(Guid id)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo == null)
            {
                return NotFound();
            }

            return Ok(photo);
        }

        /// <summary>
        /// Gets all photos for a profile
        /// </summary>
        /// <param name="profileId">The ID of the profile</param>
        [HttpGet("profile/{profileId}")]
        [ProducesResponseType(typeof(IEnumerable<Photo>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Photo>>> GetProfilePhotos(Guid profileId)
        {
            var photos = await _context.Photos
                .Where(p => p.ProfileId == profileId)
                .OrderByDescending(p => p.IsMain)
                .ThenBy(p => p.CreatedAt)
                .ToListAsync();

            return Ok(photos);
        }

        /// <summary>
        /// Deletes a photo
        /// </summary>
        /// <param name="id">The ID of the photo to delete</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePhoto(Guid id)
        {
            try
            {
                var photo = await _context.Photos.FindAsync(id);
                if (photo == null)
                {
                    return NotFound();
                }

                // Delete from S3
                var deleteSuccess = await _imageService.DeleteImageAsync(photo.Url);
                if (!deleteSuccess)
                {
                    _logger.LogWarning("Failed to delete image from S3: {ImageUrl}", photo.Url);
                }

                // Delete from database
                _context.Photos.Remove(photo);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Photo deleted successfully: {PhotoId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting photo {PhotoId}", id);
                return StatusCode(500, new { error = "Failed to delete photo", message = ex.Message });
            }
        }

        /// <summary>
        /// Sets a photo as the main photo for a profile
        /// </summary>
        /// <param name="id">The ID of the photo to set as main</param>
        [HttpPut("{id}/set-main")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetMainPhoto(Guid id)
        {
            try
            {
                var photo = await _context.Photos.FindAsync(id);
                if (photo == null)
                {
                    return NotFound();
                }

                // Unset other main photos for this profile
                var existingMainPhotos = await _context.Photos
                    .Where(p => p.ProfileId == photo.ProfileId && p.IsMain)
                    .ToListAsync();

                foreach (var existingPhoto in existingMainPhotos)
                {
                    existingPhoto.IsMain = false;
                    existingPhoto.UpdatedAt = DateTime.UtcNow;
                }

                // Set this photo as main
                photo.IsMain = true;
                photo.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Photo {PhotoId} set as main for profile {ProfileId}", id, photo.ProfileId);

                return Ok(photo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting main photo {PhotoId}", id);
                return StatusCode(500, new { error = "Failed to set main photo", message = ex.Message });
            }
        }

        /// <summary>
        /// Gets upload limits and supported formats
        /// </summary>
        [HttpGet("limits")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult GetUploadLimits()
        {
            var limits = new
            {
                MaxFileSizeBytes = _imageService.GetMaxFileSize(),
                MaxFileSizeMB = _imageService.GetMaxFileSize() / (1024 * 1024),
                SupportedFormats = new[] { "JPG", "JPEG", "PNG", "GIF", "WebP" }
            };

            return Ok(limits);
        }
    }
} 
using Microsoft.AspNetCore.Http;

namespace CompatibleAPI.Application.Interfaces
{
    /// <summary>
    /// Service interface for handling image upload and management operations
    /// </summary>
    public interface IImageService
    {
        /// <summary>
        /// Uploads an image to S3 and returns the CloudFront URL
        /// </summary>
        /// <param name="file">The image file to upload</param>
        /// <param name="profileId">The ID of the profile this image belongs to</param>
        /// <returns>The CloudFront URL of the uploaded image</returns>
        Task<string> UploadImageAsync(IFormFile file, Guid profileId);

        /// <summary>
        /// Deletes an image from S3
        /// </summary>
        /// <param name="imageUrl">The CloudFront URL of the image to delete</param>
        /// <returns>True if deletion was successful</returns>
        Task<bool> DeleteImageAsync(string imageUrl);

        /// <summary>
        /// Validates if the uploaded file is a valid image
        /// </summary>
        /// <param name="file">The file to validate</param>
        /// <returns>True if the file is a valid image</returns>
        bool IsValidImage(IFormFile file);

        /// <summary>
        /// Gets the maximum file size allowed for image uploads
        /// </summary>
        /// <returns>Maximum file size in bytes</returns>
        long GetMaxFileSize();
    }
} 
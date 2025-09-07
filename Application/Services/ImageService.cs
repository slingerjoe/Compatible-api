using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using CompatibleAPI.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace CompatibleAPI.Application.Services
{
    /// <summary>
    /// Service for handling image upload and management operations with AWS S3
    /// </summary>
    public class ImageService : IImageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ImageService> _logger;
        private readonly string _bucketName;
        private readonly string _cloudFrontUrl;
        private readonly long _maxFileSize = 10 * 1024 * 1024; // 10MB
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public ImageService(
            IAmazonS3 s3Client,
            IConfiguration configuration,
            ILogger<ImageService> logger)
        {
            _s3Client = s3Client;
            _configuration = configuration;
            _logger = logger;
            _bucketName = _configuration["AWS:S3:BucketName"] ?? "compatible-images-dev";
            _cloudFrontUrl = _configuration["AWS:CloudFront:Url"] ?? "https://d1us4yuvo02gbo.cloudfront.net/";
        }

        public async Task<string> UploadImageAsync(IFormFile file, Guid profileId)
        {
            try
            {
                if (!IsValidImage(file))
                {
                    throw new ArgumentException("Invalid image file");
                }

                // Generate unique filename
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = $"{profileId}_{Guid.NewGuid()}{fileExtension}";

                // Upload to S3
                using var transferUtility = new TransferUtility(_s3Client);
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = file.OpenReadStream(),
                    Key = fileName,
                    BucketName = _bucketName,
                    ContentType = file.ContentType
                    // Removed CannedACL since bucket doesn't allow ACLs
                };

                await transferUtility.UploadAsync(uploadRequest);

                // Return CloudFront URL
                var cloudFrontUrl = $"{_cloudFrontUrl.TrimEnd('/')}/{fileName}";
                _logger.LogInformation("Image uploaded successfully: {FileName} for profile {ProfileId}", fileName, profileId);
                
                return cloudFrontUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image for profile {ProfileId}", profileId);
                throw;
            }
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            try
            {
                // Extract filename from CloudFront URL
                var fileName = ExtractFileNameFromUrl(imageUrl);
                if (string.IsNullOrEmpty(fileName))
                {
                    _logger.LogWarning("Could not extract filename from URL: {ImageUrl}", imageUrl);
                    return false;
                }

                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileName
                };

                await _s3Client.DeleteObjectAsync(deleteRequest);
                _logger.LogInformation("Image deleted successfully: {FileName}", fileName);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image: {ImageUrl}", imageUrl);
                return false;
            }
        }

        public bool IsValidImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > _maxFileSize)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                return false;

            // Check content type
            var allowedContentTypes = new[]
            {
                "image/jpeg",
                "image/jpg",
                "image/png",
                "image/gif",
                "image/webp"
            };

            return allowedContentTypes.Contains(file.ContentType.ToLowerInvariant());
        }

        public long GetMaxFileSize()
        {
            return _maxFileSize;
        }

        private string? ExtractFileNameFromUrl(string imageUrl)
        {
            try
            {
                // Remove CloudFront base URL to get just the filename
                var baseUrl = _cloudFrontUrl.TrimEnd('/');
                if (imageUrl.StartsWith(baseUrl))
                {
                    return imageUrl.Substring(baseUrl.Length + 1); // +1 for the slash
                }

                // Fallback: try to extract filename from URL
                var uri = new Uri(imageUrl);
                return Path.GetFileName(uri.LocalPath);
            }
            catch
            {
                return null;
            }
        }
    }
} 
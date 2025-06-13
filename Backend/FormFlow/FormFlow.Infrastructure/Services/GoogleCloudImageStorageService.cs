using FormFlow.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System.Text.Json;

namespace FormFlow.Infrastructure.Services
{
    public class GoogleCloudImageStorageService : IImageStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public GoogleCloudImageStorageService(IConfiguration configuration)
        {
            var serviceAccountJson = configuration
                .GetSection("GoogleCloudStorage:ServiceAccount")
                .Get<Dictionary<string, object>>();
            var credential = GoogleCredential
                .FromJson(JsonSerializer
                .Serialize(serviceAccountJson));

            _storageClient = StorageClient.Create(credential);
            _bucketName = configuration["GoogleCloudStorage:BucketName"];
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
        {
            var objectName = $"images/{Guid.NewGuid()}_{fileName}";
            var contentType = GetContentType(fileName);

            var googleObject = await _storageClient.UploadObjectAsync(_bucketName, objectName, contentType, imageStream);

            return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
        }

        public async Task<string> UploadImageAsync(byte[] imageData, string fileName)
        {
            using var stream = new MemoryStream(imageData);
            return await UploadImageAsync(stream, fileName);
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            try
            {
                var objectName = ExtractObjectNameFromUrl(imageUrl);
                if (string.IsNullOrEmpty(objectName))
                    return false;

                await _storageClient.DeleteObjectAsync(_bucketName, objectName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ImageExistsAsync(string imageUrl)
        {
            try
            {
                var objectName = ExtractObjectNameFromUrl(imageUrl);
                if (string.IsNullOrEmpty(objectName))
                    return false;

                await _storageClient.GetObjectAsync(_bucketName, objectName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string ExtractObjectNameFromUrl(string imageUrl)
        {
            var prefix = $"https://storage.googleapis.com/{_bucketName}/";
            if (imageUrl.StartsWith(prefix))
                return imageUrl.Substring(prefix.Length);

            return string.Empty;
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            return extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}

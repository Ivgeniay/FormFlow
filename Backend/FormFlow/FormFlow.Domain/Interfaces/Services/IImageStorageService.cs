using FormFlow.Domain.Models.ImageStorage;

namespace FormFlow.Domain.Interfaces.Services
{
    public interface IImageStorageService
    {
        Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType);
        Task<string> UploadImageAsync(byte[] imageData, string fileName, string contentType);
        Task<bool> DeleteImageAsync(string imageUrl);
        Task<bool> ImageExistsAsync(string imageUrl);
        Task<string> GetImageUrlAsync(string fileName);
        Task<long> GetImageSizeAsync(string imageUrl);
        Task<ImageUploadOptionsRequest> GetImageOptions();
    }


}

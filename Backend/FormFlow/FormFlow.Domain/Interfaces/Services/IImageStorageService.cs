namespace FormFlow.Domain.Interfaces.Services
{
    public interface IImageStorageService
    {
        Task<string> UploadImageAsync(Stream imageStream, string fileName);
        Task<string> UploadImageAsync(byte[] imageData, string fileName);
        Task<bool> DeleteImageAsync(string imageUrl);
        Task<bool> ImageExistsAsync(string imageUrl);
        Task<Stream?> GetImageStreamAsync(string imageUrl);
    }


}

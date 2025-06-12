namespace FormFlow.Domain.Models.ImageStorage
{
    public class ImageUploadResult
    {
        public bool Success { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }


}

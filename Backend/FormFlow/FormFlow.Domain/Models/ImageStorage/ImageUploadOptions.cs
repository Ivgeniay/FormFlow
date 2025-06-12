namespace FormFlow.Domain.Models.ImageStorage
{
    public class ImageUploadOptions
    {
        public int MaxWidthPixels { get; set; } = 1920;
        public int MaxHeightPixels { get; set; } = 1080;
        public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;
        public List<string> AllowedContentTypes { get; set; } = new()
        {
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/webp"
        };
        public bool GenerateUniqueFileName { get; set; } = true;
        public string? Folder { get; set; } = "templates";
    }


}

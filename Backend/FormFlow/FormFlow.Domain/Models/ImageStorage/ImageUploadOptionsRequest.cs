namespace FormFlow.Domain.Models.ImageStorage
{
    public class ImageUploadOptionsRequest
    {
        public int MaxWidthPixels { get; set; }
        public int MaxHeightPixels { get; set; }
        public long MaxFileSizeBytes { get; set; }
        public List<string> AllowedTypes { get; set; } = new List<string>();
    }


}

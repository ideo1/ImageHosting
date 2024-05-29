namespace ImageHosting.Services.ImageStorageProviders
{
    public class ImageRequestModel
    {
        public string? FolderName { get; set; }
        public string? ImageName { get; set; }
        public int Height { get; set; } = 0;
        public int Width { get; set; } = 0;
    }
}

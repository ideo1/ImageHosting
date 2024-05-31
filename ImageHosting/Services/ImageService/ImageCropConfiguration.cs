namespace ImageHosting.Services.ImageService
{
    public class ImageCropConfiguration
    {
        public IEnumerable<ImageCrop> ImageCrops { get; set; }
    }
    public class ImageCrop
    {
        public string Alias { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}

namespace ImageHosting.Services.ImageService.Models
{
    public class ImageRequestModel: ImageRequestBase
    {
        public int Height { get; set; } = 0;
        public int Width { get; set; } = 0;
        public Watermark Watermark { get; set; }
    }
}

namespace ImageHosting.Services.ImageService.Models
{
    public abstract class ImageRequestBase
    {
        public string OriginalPath { get; set; }
        public string ImageName { get; set; }
        public string ImageExtension { get; set; }
    }
}

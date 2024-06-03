using SixLabors.ImageSharp;

namespace ImageHosting.Services.ImageService.Models
{
    public class ImageSaveModel: ImageRequestBase
    {
        public Image? Image { get; set; }
    }
}
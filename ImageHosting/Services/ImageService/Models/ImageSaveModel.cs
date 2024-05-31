using SixLabors.ImageSharp;

namespace ImageHosting.Services.ImageService.Models
{
    public class ImageSaveModel
    {
        public Image? Image { get; set; }
        public string OriginalPath { get; set; }
        public string ImageName { get; set; }
        public string ImageExtension { get; set; }
    }
}
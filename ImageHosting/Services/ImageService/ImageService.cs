using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ImageHosting.Services.ImageService
{
    public interface IImageService
    {
        Task CreateImageCrops(string imagePath);
    }
    public class ImageService : IImageService
    {
        private readonly ImageServiceConfiguration _imageServiceConfiguration;
        public ImageService(ImageServiceConfiguration imageServiceConfiguration)
        {
            _imageServiceConfiguration = imageServiceConfiguration;
        }
        public async Task CreateImageCrops(string imagePath)
        {
            if (_imageServiceConfiguration.ImageCrops == null || !_imageServiceConfiguration.ImageCrops.Any())
            {
                return;
            }

            string localImagePath = Path.Combine(Directory.GetCurrentDirectory(), "StaticImages", imagePath);
            var taskList = _imageServiceConfiguration.ImageCrops.Select(async crop =>
            {
                using var image = await Image.LoadAsync(localImagePath);

                image.Mutate(x => x.Resize(crop.Width, crop.Height));

                string newImagePath = Path.Combine(Directory.GetCurrentDirectory(), "StaticImages", $"{crop.Alias}.png");
                await image.SaveAsync(newImagePath);
            }).ToArray();

            await Task.WhenAll(taskList);
        }
    }
}

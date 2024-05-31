using ImageHosting.Services.ImageService.Models;
using ImageHosting.Services.ImageStorageProviders;
using SixLabors.ImageSharp;
using System.Net;

namespace ImageHosting.Services.ImageService
{
    public interface IImageService
    {
        Task CreateImageCrops(string imagePath);
    }
    public class ImageService : IImageService
    {
        private readonly ImageCropConfiguration _imageServiceConfiguration;
        private readonly IImageStorageProvider _imageStorageProvider;
        private readonly HttpClient _httpClient;
        public ImageService(ImageCropConfiguration imageServiceConfiguration,
                            HttpClient httpClient,
                            IImageStorageProvider imageStorageProvider)
        {
            _imageServiceConfiguration = imageServiceConfiguration;
            _httpClient = httpClient;
            _imageStorageProvider = imageStorageProvider;
        }
        public async Task CreateImageCrops(string imagePath)
        {
            if (_imageServiceConfiguration.ImageCrops == null || !_imageServiceConfiguration.ImageCrops.Any())
            {
                return;
            }

            try
            {
                using HttpResponseMessage response = await _httpClient.GetAsync(imagePath);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    //LogDefineOptions error
                }

                using var stream = new MemoryStream(await response.Content.ReadAsByteArrayAsync());
                using var image = Image.Load(stream);
                var pathSegments = imagePath.Split('/');
                var imageName = pathSegments.Last().Split('.').FirstOrDefault();
                var extension = Path.GetExtension(imagePath);

                var imageToSave = new ImageSaveModel()
                {
                    Image = image,
                    OriginalPath = imagePath,
                    ImageName = imageName,
                    ImageExtension = extension
                };

                 await _imageStorageProvider.SaveImage(imageToSave);
            }
            catch (Exception ex)
            {

            }
        }
    }
}

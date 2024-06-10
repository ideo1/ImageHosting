using ImageHosting.Services.ImageService.Models;
using ImageHosting.Services.ImageStorageProviders;
using SixLabors.ImageSharp;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace ImageHosting.Services.ImageService
{
    public interface IImageService
    {
        Task CreateImageCrops(string imagePath);
        Task<byte[]?> GetImage(string imagePath);
    }
    public class ImageService : IImageService
    {
        private const string _imageNamePattern = @"(?<filename>.+)\.(?<ext>[^.?]+)(?:\?width=(?<width>\d+))?(?:&height=(?<height>\d+))?";
        private readonly ImageCropConfiguration _imageCropConfiguration;
        private readonly IImageStorageProvider _imageStorageProvider;
        private readonly ImageServiceConfiguration _imageServiceConfiguration;
        private readonly HttpClient _httpClient;
        public ImageService(ImageCropConfiguration imageCropConfiguration,
                            HttpClient httpClient,
                            IImageStorageProvider imageStorageProvider,
                            ImageServiceConfiguration imageServiceConfiguration)
        {
            _imageCropConfiguration = imageCropConfiguration;
            _httpClient = httpClient;
            _imageStorageProvider = imageStorageProvider;
            _imageServiceConfiguration = imageServiceConfiguration;
        }
        public async Task CreateImageCrops(string imagePath)
        {
            if (_imageCropConfiguration.ImageCrops == null || !_imageCropConfiguration.ImageCrops.Any())
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

        public async Task<byte[]?> GetImage(string imagePath)
        {
            var decodedPath = HttpUtility.UrlDecode(imagePath);
            var imageNameSegment = decodedPath.Split('/').LastOrDefault();
            Match match = Regex.Match(imageNameSegment, _imageNamePattern);

            if (!match.Success)
            {
                return null;
            }

            // Extract data
            string fileName = match.Groups["filename"].Value;
            string extension = match.Groups["ext"].Value;
            int width = match.Groups["width"].Success ? int.Parse(match.Groups["width"].Value) : 0; // 0 indicates width not specified
            int height = match.Groups["height"].Success ? int.Parse(match.Groups["height"].Value) : 0; // 0 indicates height not specified
            var model = new ImageRequestModel()
            {
                Height = height,
                Width = width,
                ImageExtension = string.Format(".{0}", extension),
                ImageName = fileName,
                OriginalPath = imagePath
            };

            var res = await _imageStorageProvider.GetImage(model);

            if (res != null && !_imageServiceConfiguration.CreateIfNotExists)
            {
                return res;
            }

            await CreateImageCrops(imagePath);
            res = await _imageStorageProvider.GetImage(model);

            return res;
        }
    }
}

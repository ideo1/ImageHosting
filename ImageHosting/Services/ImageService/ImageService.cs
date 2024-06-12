using ImageHosting.Services.ImageService.Models;
using ImageHosting.Services.ImageStorageProviders;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
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
        Task ImportImagesWithCrops(int pageNumber, CancellationToken cancellationToken);
    }
    public class ImageService : IImageService
    {
        private const string _imageNamePattern = @"(?<filename>.+)\.(?<ext>[^.?]+)(?:\?width=(?<width>\d+))?(?:&height=(?<height>\d+))?";
        private readonly ImageCropConfiguration _imageCropConfiguration;
        private readonly IImageStorageProvider _imageStorageProvider;
        private readonly ImageServiceConfiguration _imageServiceConfiguration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ImageService> _logger;
        public ImageService(ImageCropConfiguration imageCropConfiguration,
                            HttpClient httpClient,
                            IImageStorageProvider imageStorageProvider,
                            ImageServiceConfiguration imageServiceConfiguration,
                            ILogger<ImageService> logger)
        {
            _imageCropConfiguration = imageCropConfiguration;
            _httpClient = httpClient;
            _imageStorageProvider = imageStorageProvider;
            _imageServiceConfiguration = imageServiceConfiguration;
            _logger = logger;
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
                    _logger.LogError($"{nameof(CreateImageCrops)} http request error. Error code {response.StatusCode}. Image path {imagePath}");

                    return;
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
                _logger.LogError($"{nameof(CreateImageCrops)} exception. Error message {ex.Message}.");
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

            if (res != null || !_imageServiceConfiguration.CreateIfNotExists)
            {
                return res;
            }

            await CreateImageCrops(imagePath);
            res = await _imageStorageProvider.GetImage(model);

            return res;
        }

        public async Task ImportImagesWithCrops(int pageNumber, CancellationToken cancellationToken)
        {
            var queryParameters = new List<KeyValuePair<string, string>>
            {
                 new KeyValuePair<string, string>("pageNumber", pageNumber.ToString())
            };

            var queryString = QueryHelpers.AddQueryString(_imageServiceConfiguration.UmbracoSettings.MigrationEndpoint, queryParameters);
            using HttpResponseMessage response = await _httpClient.GetAsync(queryString);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"{nameof(ImportImagesWithCrops)} http request error. Error code {response.StatusCode}. Page number {pageNumber}");

                return;
            }

            var res = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(res))
            {
                _logger.LogWarning($"{nameof(ImportImagesWithCrops)} response is empty. Page number {pageNumber}");

                return;
            }

            var responseData = JsonConvert.DeserializeObject<ImageMigrationResponseModel>(res);

            if (responseData == null || responseData.ImageUrls == null || !responseData.ImageUrls.Any())
            {
                _logger.LogWarning($"{nameof(ImportImagesWithCrops)} response has no element. Page number {pageNumber}");

                return;
            }

            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = _imageServiceConfiguration.UmbracoSettings.MaxDegreeOfParallelism
            };

            await Parallel.ForEachAsync(responseData.ImageUrls, options, async (url, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                await CreateImageCrops(url);
            });
        }
    }
}

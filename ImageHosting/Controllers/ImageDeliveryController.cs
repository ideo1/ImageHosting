using ImageHosting.Filters;
using ImageHosting.Services.ImageService;
using ImageHosting.Services.ImageStorageProviders;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace ImageHosting.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ImageDeliveryController : ControllerBase
    {
        private readonly ILogger<ImageDeliveryController> _logger;
        private readonly IImageService _imageService;
        private readonly IImageStorageProvider _imageStorageProvider;

        public ImageDeliveryController(ILogger<ImageDeliveryController> logger,
                                       IImageService imageService,
                                       IImageStorageProvider imageStorageProvider)
        {
            _logger = logger;
            _imageService = imageService;
            _imageStorageProvider = imageStorageProvider;
        }
        [HttpGet]
        /// <summary>
        /// Returns image for requested path to umbraco media folder.
        /// </summary>
        /// <param name="imagePath">The relative path to the image in the format /folderName/imageName?width=100&wiight=100>.</param>
        /// <returns>The image <see cref="ActionResult"/> for requested path.</returns>
        public async Task<ActionResult> GetImage(string imagePath)
        {
            return NotFound();
            if (string.IsNullOrEmpty(imagePath))
            {
                return NotFound();
            }

            var urlParts = imagePath.Split(new char[] { '/'}, StringSplitOptions.RemoveEmptyEntries);

            if (urlParts.Length < 2)
            {
                return BadRequest();
            }

            var queryParts = urlParts.LastOrDefault(imagePath).Split(new char[] { '?' }, StringSplitOptions.RemoveEmptyEntries);
            var request = new ImageRequestModel()
            {
                FolderName = urlParts.FirstOrDefault(),
                ImageName =  HttpUtility.UrlDecode(queryParts.FirstOrDefault())
            };

            if (queryParts.Length >1)
            {
                var queryParameters = HttpUtility.ParseQueryString(queryParts.LastOrDefault());
                int.TryParse(queryParameters.Get("width"), out var width);
                int.TryParse(queryParameters.Get("height"), out var height);
                request.Height = height;
                request.Width = width;
            }

            var image = await _imageStorageProvider.GetImage(request);

            if (image == null)
            {
                return NotFound();
            }

            return File(image, GetContentType(imagePath), imagePath);
        }

        [HttpGet]
        [ImageProcessAuthorizationAttribute]
        public async Task AddImageCrops(string imagePath)
        {
            await _imageService.CreateImageCrops(imagePath);
        }

        private string GetContentType(string filePath)
        {
            string? extension = Path.GetExtension(filePath)?.ToLowerInvariant();

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".bmp":
                    return "image/bmp";
                default:
                    return "application/octet-stream";
            }
        }
    }
}

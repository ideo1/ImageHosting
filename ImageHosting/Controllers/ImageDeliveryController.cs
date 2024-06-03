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

        public ImageDeliveryController(ILogger<ImageDeliveryController> logger,
                                       IImageService imageService,
                                       IImageStorageProvider imageStorageProvider)
        {
            _logger = logger;
            _imageService = imageService;
        }
        [HttpGet]
        /// <summary>
        /// Returns image for requested path to umbraco media folder.
        /// </summary>
        /// <param name="imagePath">The relative path to the image in the format /folderName/imageName?width=100&wiight=100>.</param>
        /// <returns>The image <see cref="ActionResult"/> for requested path.</returns>
        public async Task<ActionResult> GetImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return NotFound();
            }
            
            var image = await _imageService.GetImage(imagePath);

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

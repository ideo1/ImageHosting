using ImageHosting.Extensions;
using ImageHosting.Filters;
using ImageHosting.Services.ImageService;
using ImageHosting.Services.ImageService.Models;
using ImageHosting.Services.ImageStorageProviders;
using Microsoft.AspNetCore.Mvc;

namespace ImageHosting.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ImageDeliveryController : ControllerBase
    {
        private readonly ILogger<ImageDeliveryController> _logger;
        private readonly IImageService _imageService;
        private readonly ImageServiceConfiguration _imageServiceConfiguration;

        public ImageDeliveryController(ILogger<ImageDeliveryController> logger,
                                       IImageService imageService,
                                       IImageStorageProvider imageStorageProvider,
                                       ImageServiceConfiguration imageServiceConfiguration)
        {
            _logger = logger;
            _imageService = imageService;
            _imageServiceConfiguration = imageServiceConfiguration;
        }
        [HttpGet]
        /// <summary>
        /// Returns image for requested path to umbraco media folder.
        /// </summary>
        /// <param name="imagePath">The relative path to the image in the format /media/folderName/imageName?width=100&wiight=100.</param>
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

            return File(image, imagePath.GetContentType(), imagePath);
        }

        [HttpGet]
        [ImageProcessAuthorizationAttribute]
        public async Task AddImageCrops(string imagePath)
        {
            await _imageService.CreateImageCrops(imagePath);
        }
        [HttpGet]
        /// <summary>
        /// Fetch images from umbraco site and create image crops.
        /// </summary>
        /// <param name="requestModel">Need to provide StartPageNumber (0 or greater) and  NumberOfPagesToProcess (1 or greater, 1 is default0</param>
        /// <returns>Status of the process <see cref="ActionResult"/> for requested import process.</returns>
        public async Task<ActionResult> ImportImages([FromQuery]ImageMigrationRequestModel requestModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid || !_imageServiceConfiguration.UmbracoSettings.AllowMigration)
            {
                return BadRequest(ModelState);
            }

            var migrationId = Guid.NewGuid().ToString();

            try
            {
                _logger.LogWarning($"{nameof(ImportImages)} image migration process started. Migration id {migrationId}. Start with page {requestModel.StartPageNumber}. Number of pages to migrate {requestModel.NumberOfPagesToProcess}");
                for (int i = 0; i < requestModel.NumberOfPagesToProcess; i++)
                {
                    var currentPageNumber = requestModel.StartPageNumber + i;
                    cancellationToken.ThrowIfCancellationRequested();
                    await _imageService.ImportImagesWithCrops(currentPageNumber, cancellationToken);
                }

                _logger.LogWarning($"{nameof(ImportImages)} image migration process successfully finished. Migration id {migrationId}.");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(ImportImages)} error. Migration id {migrationId}. Error message {ex.Message}.");

                return StatusCode(500, ex.Message);
            }
        }
    }
}

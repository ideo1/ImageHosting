﻿using ImageHosting.Extensions;
using ImageHosting.Filters;
using ImageHosting.Services.ImageService;
using ImageHosting.Services.ImageService.Models;
using ImageHosting.Services.ImageStorageProviders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

            return File(image, imagePath.GetContentType(), imagePath);
        }

        [HttpGet]
        [ImageProcessAuthorizationAttribute]
        public async Task AddImageCrops(string imagePath)
        {
            await _imageService.CreateImageCrops(imagePath);
        }
        [HttpGet]
        public async Task<ActionResult> ImportImages([FromQuery]ImageMigrationRequestModel requestModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var options = new ParallelOptions()
            //{
            //    MaxDegreeOfParallelism = 20
            //};

            try
            {
                return Ok();
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
    }
}

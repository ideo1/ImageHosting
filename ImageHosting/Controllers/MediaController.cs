using ImageHosting.Extensions;
using ImageHosting.Services.ImageService;
using Microsoft.AspNetCore.Mvc;

namespace ImageHosting.Controllers
{
    [ApiController]
    [Route("media")]
    public class MediaController: ControllerBase
    {
        private readonly IImageService _imageService;
        public MediaController(IImageService imageService)
        {
            _imageService = imageService;
        }
        [HttpGet("{*path}")]
        public async Task<IActionResult> ProcessMedia(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return NotFound();
            }

            var image = await _imageService.GetImage(Uri.UnescapeDataString(Request.Path));

            if (image == null)
            {
                return NotFound();
            }

            return File(image, path.GetContentType(), path);
        }
    }
}

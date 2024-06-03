using ImageHosting.Services.ImageService.Models;

namespace ImageHosting.Services.ImageStorageProviders
{
    public interface IImageStorageProvider
    {
        Task<byte[]?> GetImage(ImageRequestModel model);
        Task SaveImage(ImageSaveModel model);
    }
}

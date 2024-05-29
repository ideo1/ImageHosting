using ImageHosting.Extensions;
using ImageHosting.Services.ImageService;

namespace ImageHosting.Services.ImageStorageProviders
{
    public class LocalStorageProvider : IImageStorageProvider
    {
        private readonly ImageServiceConfiguration _imageServiceConfiguration;

        public LocalStorageProvider(ImageServiceConfiguration imageServiceConfiguration)
        {
            _imageServiceConfiguration = imageServiceConfiguration;
        }

        public async Task<byte[]?> GetImage(ImageRequestModel request)
        {
            try
            {
                var imageNameWithCrops = request.GetImageNameWithCrops();
                var imageBytes = await GetImageByName(request.FolderName, imageNameWithCrops);

                if (imageBytes != null)
                {
                    return imageBytes;
                }

                imageBytes = await GetImageByName(request.FolderName, request.ImageName);

                return imageBytes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<byte[]?> GetImageByName(string folderName, string imageName)
        {
            try
            {
                string localImagePath = Path.Combine(_imageServiceConfiguration.LocalStorageSettings.LocalFolderPath, folderName, imageName);
                byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(localImagePath);

                return imageBytes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

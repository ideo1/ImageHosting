using ImageHosting.Extensions;
using ImageHosting.Services.ImageService;
using ImageHosting.Services.ImageService.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ImageHosting.Services.ImageStorageProviders
{
    public class LocalStorageProvider : IImageStorageProvider
    {
        private readonly ImageServiceConfiguration _imageServiceConfiguration;
        private readonly ImageCropConfiguration _imageCropConfiguration;

        public LocalStorageProvider(ImageServiceConfiguration imageServiceConfiguration,
                                    ImageCropConfiguration imageCropConfiguration)
        {
            _imageServiceConfiguration = imageServiceConfiguration;
            _imageCropConfiguration = imageCropConfiguration;
        }
        #region public methods
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

        public async Task SaveImage(ImageSaveModel model)
        {
            if (model.Image == null)
            {
                return;
            }

            var folder = GetFolder(model.OriginalPath);

            var taskList = _imageCropConfiguration.ImageCrops.Select(async crop =>
            {
                model.Image.Mutate(x => x.Resize(crop.Width, crop.Height));
                string fileName = string.Format("{0}_width={1}_height={2}.{3}", model.ImageName, crop.Width, crop.Height, model.ImageExtension);
                string newImagePath = Path.Combine(Directory.GetCurrentDirectory(), folder, fileName);
                await model.Image.SaveAsync(newImagePath);
            }).ToArray();

            await Task.WhenAll(taskList);
        }
        #endregion
        #region private methods
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

        private string GetFolder(string path)
        {
            var folders = new List<string>
            {
                _imageServiceConfiguration.LocalStorageSettings.LocalFolderPath
            };

            folders.AddRange(path.Split('/')[..^1]);
            var folderPath = Path.Combine(folders.ToArray());

            if (Directory.Exists(folderPath))
            {
                return folderPath;
            }

            Directory.CreateDirectory(folderPath);

            return folderPath;
        }
        #endregion
    }
}

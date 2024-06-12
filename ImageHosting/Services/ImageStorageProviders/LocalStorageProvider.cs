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
        public async Task<byte[]?> GetImage(ImageRequestModel model)
        {
            try
            {
                var folder = GetFolder(model.OriginalPath);
                var imageStorageFileName = ImageNameExtension.GetImageNameWithCrops(Path.Combine(folder, model.ImageName), model.Width, model.Height, model.ImageExtension);

                var imageBytes = await System.IO.File.ReadAllBytesAsync(imageStorageFileName);

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
            var originalFilePath = Path.Combine(folder, string.Format("{0}{1}", model.ImageName, model.ImageExtension));

            if (File.Exists(originalFilePath))
            {
                return;
            }

            //need to save original image also
            await model.Image.SaveAsync(originalFilePath); 
            
            var taskList = _imageCropConfiguration.ImageCrops.Select(async crop =>
            {
                using var copy = model.Image.Clone(x => x.Resize(crop.Width, crop.Height));
                string fileName = ImageNameExtension.GetImageNameWithCrops(model.ImageName, crop.Width, crop.Height, model.ImageExtension);
                string newImagePath = Path.Combine(folder, fileName);
                await copy.SaveAsync(newImagePath);
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

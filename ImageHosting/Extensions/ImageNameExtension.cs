using ImageHosting.Services.ImageStorageProviders;
using System.Reflection;

namespace ImageHosting.Extensions
{
    public static class ImageNameExtension
    {
        public static string GetImageNameWithCrops(string imageName, int width, int height, string extension) =>
            string.Format("{0}_width={1}_height={2}{3}", imageName, width, height, extension);
    }
}

using ImageHosting.Services.ImageService;
namespace ImageHosting.Extensions
{
    public static class ImageNameExtension
    {
        public static string GetImageNameWithCrops(string imageName, int width, int height, string extension, Watermark watermark = null)
        {
            if (width == 0 && height == 0)
            {
                return string.Format("{0}{1}", imageName, extension);
            }

            if(watermark == null)
            {
                return string.Format("{0}_width={1}_height={2}{3}", imageName, width, height, extension);
            }

            return string.Format("{0}_width={1}_height={2}_{3}={4}{5}", imageName, width, height, watermark.QuerySegmentName, watermark.ImageName, extension);
        }

        public static string GetContentType(this string filePath)
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

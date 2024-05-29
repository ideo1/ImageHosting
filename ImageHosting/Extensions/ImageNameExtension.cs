using ImageHosting.Services.ImageStorageProviders;

namespace ImageHosting.Extensions
{
    public static class ImageNameExtension
    {
        public static string GetImageNameWithCrops(this ImageRequestModel request)
        {
            if (request == null || string.IsNullOrEmpty(request.ImageName))
            {
                return string.Empty;
            }

            if (request.Height == 0 && request.Width == 0)
            {
                return request.ImageName;
            }

            var imageNameWithExtension = request.ImageName.Split('.');

            if (imageNameWithExtension.Length !=2)
            {
                return request.ImageName;
            }

            return string.Format("{0}_w{1}_h{2}.{3}", imageNameWithExtension.FirstOrDefault(), request.Width, request.Height, imageNameWithExtension.LastOrDefault());
        }
    }
}

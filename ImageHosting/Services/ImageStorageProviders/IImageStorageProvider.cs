namespace ImageHosting.Services.ImageStorageProviders
{
    public interface IImageStorageProvider
    {
        Task<byte[]?> GetImage(ImageRequestModel request);
    }
}

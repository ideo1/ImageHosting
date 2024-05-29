namespace ImageHosting.Services.ImageService
{
    public class ImageServiceConfiguration
    {
        public bool UseLocalStorage { get; set; }
        public LocalStorageSettings LocalStorageSettings { get; set; }
        public IEnumerable<ImageCrop> ImageCrops { get; set; }
    }
    public class ImageCrop
    {
        public string Alias { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
    public class LocalStorageSettings
    {
        public string LocalFolderPath { get; set; }
    }
}

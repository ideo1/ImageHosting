namespace ImageHosting.Services.ImageService
{
    public class ImageServiceConfiguration
    {
        public const string ConfigurationName = "ImageService";
        public bool UseLocalStorage { get; set; }
        public bool CreateIfNotExists { get; set; }
        public LocalStorageSettings LocalStorageSettings { get; set; }
        public UmbracoSettings UmbracoSettings { get; set; }
        public Authorize Authorize { get; set; }
    }

    public class Authorize
    {
        public string Token { get; set; }
    }

    public class LocalStorageSettings
    {
        public string LocalFolderPath { get; set; }
    }
    public class UmbracoSettings
    {
        public string UmbracoHostName { get; set; }
        public string RequestHeader { get; set; }
    }
}

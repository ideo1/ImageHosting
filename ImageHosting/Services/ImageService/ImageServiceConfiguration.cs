namespace ImageHosting.Services.ImageService
{
    public class ImageServiceConfiguration
    {
        public const string ConfigurationName = "ImageService";
        public bool UseLocalStorage { get; set; }
        //if image was not imported we will try to create it
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
        public string MigrationEndpoint { get; set; }
        public int MaxDegreeOfParallelism { get; set; }
        //set to false after migration will be completed
        public bool AllowMigration { get; set; }
    }
}

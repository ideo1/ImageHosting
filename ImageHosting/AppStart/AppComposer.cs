using ImageHosting.Services.ImageService;
using ImageHosting.Services.ImageStorageProviders;
using Newtonsoft.Json;
using System.Reflection;
using System.Security.Authentication;

namespace ImageHosting.AppStart
{
    public static class AppComposer
    {
        public static void AddServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddTransient<IImageService, ImageService>();

            services.AddScoped<ImageCropConfiguration>(
                factory =>
                {
                    var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Services", "ImageService", "Json", "imageCropConfiguration.json");
                    using var reader = new StreamReader(fileName);
                    string json = reader.ReadToEnd();
                    var configuration = JsonConvert.DeserializeObject<ImageCropConfiguration>(json);

                    return configuration;
                }
           );

            var imageServiceConfiguration = configuration.GetSection(ImageServiceConfiguration.ConfigurationName)
                 .Get<ImageServiceConfiguration>();

            services.AddScoped<ImageServiceConfiguration>(
             factory =>
             {
                 return imageServiceConfiguration;
             });

            if (imageServiceConfiguration.UseLocalStorage)
            {
                services.AddTransient<IImageStorageProvider, LocalStorageProvider>();
            }

            services.AddHttpClient<IImageService, ImageService>(client =>
            {
                client.BaseAddress = new System.Uri(imageServiceConfiguration.UmbracoSettings.UmbracoHostName);
                client.DefaultRequestHeaders.Add(imageServiceConfiguration.UmbracoSettings.RequestHeader, "application/json");

            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                // Configure SSL options to only accept TLS 1.2 or higher
                handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;

                return handler;
            });
        }
    }
}
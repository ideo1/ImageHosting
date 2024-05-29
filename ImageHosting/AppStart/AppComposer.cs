using ImageHosting.Services.ImageService;
using ImageHosting.Services.ImageStorageProviders;
using Newtonsoft.Json;
using System.Reflection;

namespace ImageHosting.AppStart
{
    public static class AppComposer
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IImageService, ImageService>();

            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Services", "ImageService", "Json", "imageServiceConfiguration.json");
            using var reader = new StreamReader(fileName);
            string json = reader.ReadToEnd();
            var configuration = JsonConvert.DeserializeObject<ImageServiceConfiguration>(json);

            services.AddScoped<ImageServiceConfiguration>(
                factory =>
                {                  
                    return configuration;
                }
           );

            if (configuration.UseLocalStorage)
            {
                services.AddTransient<IImageStorageProvider, LocalStorageProvider>();
            }
        }

    }
}

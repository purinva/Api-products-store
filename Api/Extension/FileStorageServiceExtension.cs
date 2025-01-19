using Api.Service.Storage;

namespace Api.Extension
{
    public static class FileStorageServiceExtension
    {
        public static IServiceCollection AddFileStorageService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.Configure<TimeWebSettings>(configuration.GetSection("TimeWebS3"));
            return services;
        }
    }
}

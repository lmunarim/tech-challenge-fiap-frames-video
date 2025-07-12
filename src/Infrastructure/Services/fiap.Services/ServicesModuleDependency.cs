using fiap.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace fiap.Services
{
    public static class ServicesModuleDependency
    {
        public static void AddServicesModule(this IServiceCollection services)
        {
            services.AddTransient<IStatusUploadService, StatusUploadService>();
            services.AddTransient<IS3Service, S3Service>();
        }
    }
}

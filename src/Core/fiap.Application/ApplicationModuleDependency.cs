using fiap.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace fiap.Application
{
    public static class ApplicationModuleDependency
    {
        public static void AddApplicationModule(this IServiceCollection services)
        {
            services.AddTransient<IVideoUploadApplication, VideoUploadApplication>();
        }
    }
}

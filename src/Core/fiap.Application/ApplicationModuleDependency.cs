using fiap.Application.Interfaces;
using fiap.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace fiap.Application
{
    public static class ApplicationModuleDependency
    {
        public static void AddApplicationModule(this IServiceCollection services)
        {
            services.AddSingleton<IClienteApplication, ClienteApplication>();
        }
    }
}

using Amazon;
using Amazon.SecretsManager;
using fiap.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace fiap.Services
{
    public static class ServicesModuleDependency
    {
        public static void AddServicesModule(this IServiceCollection services)
        {
            services.AddSingleton<IAmazonSecretsManager>(new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName("us-east-1")));
            services.AddSingleton<ISecretManagerService, SecretManagerService>();
        }
    }
}

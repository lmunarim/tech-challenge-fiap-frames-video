using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using fiap.Domain.Interfaces;
using Serilog;
using System.Text.Json;

namespace fiap.Services
{
    public class SecretManagerService : ISecretManagerService
    {
        private readonly IAmazonSecretsManager _secret;
        private readonly ILogger _logger;
        public SecretManagerService(ILogger logger ,  IAmazonSecretsManager secret)
        {
            _secret = secret;
            _logger = logger;
        }
        public async Task<T> ObterSecret<T>(string segredo)
        {
            GetSecretValueRequest request = new GetSecretValueRequest
            {
                SecretId = segredo,
                VersionStage = "AWSCURRENT"
            };

            GetSecretValueResponse response;

            try
            {
                response = await _secret.GetSecretValueAsync(request);
                _logger.Information("Secret obtida com sucesso");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Erro ao obter a secret");
                throw;
            }

            return JsonSerializer.Deserialize<T>(response.SecretString);
        }
    }
}

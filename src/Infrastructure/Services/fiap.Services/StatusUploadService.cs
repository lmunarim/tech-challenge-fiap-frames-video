using Amazon.SecretsManager;
using Amazon.SQS;
using Amazon.SQS.Model;
using fiap.Domain.Entities;
using fiap.Domain.Interfaces.Services;
using Serilog;
using System.Text.Json;

namespace fiap.Services
{
    public class StatusUploadService : IStatusUploadService
    {
        private readonly ILogger _logger;
        public StatusUploadService(ILogger logger) { 
            _logger = logger;
        }
        public async Task EnviarStatusAsync(VideoUpload videoUpload)
        {
            try
            {
                var sqsClient = new AmazonSQSClient();
                var sendRequest = new SendMessageRequest
                {
                    QueueUrl = "https://sqs.us-east-1.amazonaws.com/147997141255/tech-challenge-fiap-upload-notifications",
                    MessageBody = JsonSerializer.Serialize(videoUpload)
                };

                var response = await sqsClient.SendMessageAsync(sendRequest);

                _logger.Information($"Video Id {videoUpload.Id}, enviado para fila com status {videoUpload.StatusUpload} -> {response.HttpStatusCode} - {response.MessageId}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Erro ao salvar na fila  - {ex.Message} - {ex.InnerException.Message}");
            }
        }
    }
}

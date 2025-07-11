using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.SecretsManager;
using fiap.Domain.Interfaces.Services;
using Serilog;

namespace fiap.Services
{
    public class UploadArquivoService : IUploadArquivoService
    {
        private const string BUCKET_NAME = "tech-challenge-fiap-7p0u9i0h";
        private readonly IAmazonSecretsManager _secret;
        private readonly ILogger _logger;
        public UploadArquivoService(ILogger logger) { 
            _logger = logger;
        }
        public async Task<string> UploadAsync(string filePath)
        {
            try
            {
                var s3Client = new AmazonS3Client(RegionEndpoint.USEast1);
                var transferUtility = new TransferUtility(s3Client);

                var request = new TransferUtilityUploadRequest
                {
                    FilePath = filePath,
                    BucketName = BUCKET_NAME,
                    Key = Path.GetFileName(filePath),
                    CannedACL = S3CannedACL.PublicRead
                };

                await transferUtility.UploadAsync(filePath, BUCKET_NAME);
                _logger.Information($"Sucesso no upload do arquivo {filePath}");

                return $"https://{BUCKET_NAME}.s3.amazonaws.com/{Path.GetFileName(filePath)}";
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Erro no upload do arquivo  - {ex.Message} - {ex.InnerException.Message}");
                throw;
            }
        }
    }
}

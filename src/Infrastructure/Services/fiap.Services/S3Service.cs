using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using fiap.Domain.Interfaces.Services;
using Serilog;

namespace fiap.Services
{
    public class S3Service : IS3Service
    {
        private const string BUCKET_NAME = "tech-challenge-fiap-7p0u9i0h";
        private readonly ILogger _logger;
        private readonly IAmazonS3 _s3Client;
        public S3Service(ILogger logger, IAmazonS3 s3Client) { 
            _logger = logger;
            _s3Client = s3Client;
        }
        public async Task<string> UploadAsync(string filePath)
        {
            try
            {
                var transferUtility = new TransferUtility(_s3Client);

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

        public async Task<(Stream FileStream, string ContentType)> DownloadAsync(string fileName)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = BUCKET_NAME,
                    Key = fileName
                };

                var response = await _s3Client.GetObjectAsync(request);
                return (response.ResponseStream, response.Headers["Content-Type"]);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Erro no upload do arquivo  - {ex.Message} - {ex.InnerException.Message}");
                throw;
            }
        }
    }
}
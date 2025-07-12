using Amazon.S3.Model;
using System.IO;
using System.Threading.Tasks;

namespace fiap.Domain.Interfaces.Services
{
    public interface IS3Service
    {
        Task<string> UploadAsync(string filePath);
        Task<(Stream FileStream, string ContentType)> DownloadAsync(string fileName);
    }
}

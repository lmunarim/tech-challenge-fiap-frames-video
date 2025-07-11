using System.Threading.Tasks;

namespace fiap.Domain.Interfaces.Services
{
    public interface IUploadArquivoService
    {
        Task<string> UploadAsync(string filePath);
    }
}

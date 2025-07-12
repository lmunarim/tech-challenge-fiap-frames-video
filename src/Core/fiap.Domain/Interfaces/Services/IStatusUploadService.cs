using fiap.Domain.Entities;
using System.Threading.Tasks;

namespace fiap.Domain.Interfaces.Services
{
    public interface IStatusUploadService
    {
        Task EnviarStatusAsync(VideoUpload videoUpload);
    }
}

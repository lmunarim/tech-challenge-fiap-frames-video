using fiap.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace fiap.Application.Interfaces
{
    public interface IVideoUploadApplication
    {
        Task ConverterVideoFrames(IFormFile video, VideoUpload videoUpload);
    }
}

using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.SQS;
using Amazon.SQS.Model;
using FFMpegCore;
using fiap.Application;
using fiap.Application.Interfaces;
using fiap.Domain.Entities;
using fiap.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Text.Json;

namespace fiap.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class VideoUploadConvertController : ControllerBase
    {
        private readonly Serilog.ILogger _logger;
        private readonly IStatusUploadService _statusUploadService;
        private readonly IVideoUploadApplication _videoUploadApplication;
        private readonly IS3Service _s3;
        public VideoUploadConvertController(Serilog.ILogger logger, 
            IStatusUploadService statusUploadService, 
            IVideoUploadApplication videoUploadApplication,
            IS3Service s3)
        {
            _logger = logger;
            _statusUploadService = statusUploadService;
            _videoUploadApplication = videoUploadApplication;
            _s3 = s3;
        }

        [HttpPost("Upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(IFormFile video, string nome ,string email)
        {
            var idVideo = Guid.NewGuid().ToString();
            var videoUpload = new VideoUpload
            {
                Id = idVideo,
                NomeArquivoOrigem = video.FileName,
                NomeArquivoGerado = $"{idVideo}.zip",
                StatusUpload = StatusUpload.Enviado,
                UrlS3 = $"https://tech-challenge-fiap-7p0u9i0h.s3.us-east-1.amazonaws.com/{idVideo}.zip",
                Usuario = new Usuario
                {
                    Nome = nome,
                    Email = email
                },
            };
            _logger.Information($"Iniciando o upload do vídeo {videoUpload.NomeArquivoOrigem} com ID {videoUpload.Id}");
            try
            {
                var ext = Path.GetExtension(video.FileName).ToLower();
                var allowed = new[] { ".mp4", ".avi", ".mov", ".mkv", ".flv", ".wmv", ".webm" };
                if (!allowed.Contains(ext))
                    return BadRequest(new { success = false, message = "Unsupported format" });

                await _statusUploadService.EnviarStatusAsync(videoUpload);

                if (video == null || video.Length == 0)
                {
                    videoUpload.StatusUpload = StatusUpload.ErroProcessamento;
                    await _statusUploadService.EnviarStatusAsync(videoUpload);

                    return BadRequest(new { success = false, message = "No video uploaded" });
                }

                _ = Task.Run( async() => await _videoUploadApplication.ConverterVideoFrames(video, videoUpload));


                return new CreatedResult(videoUpload.UrlS3, new
                {
                    success = true,
                    zip = videoUpload.NomeArquivoGerado,
                    Url = videoUpload.UrlS3
                });
            }
            catch (Exception ex)
            {
                videoUpload.StatusUpload = StatusUpload.ErroProcessamento;
                await _statusUploadService.EnviarStatusAsync(videoUpload);

                _logger.Error($"Erro {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message, innerException = ex.InnerException });
            }
        }

        [HttpGet("BaixarZip")]
        public async Task<IActionResult> BaixarZip(string fileName)
        {

            var (stream, contentType) = await _s3.DownloadAsync(fileName);
            if (stream == null)
                return NotFound("Arquivo não encontrado.");

            return File(stream, contentType, fileName);
        }

        [HttpGet("Status")]
        public Task<IActionResult> Status(string filename)
        {
            var files = Directory.GetFiles(@"/app/outputs", "*.zip");
            var list = files.Select(f =>
            {
                var info = new FileInfo(f);
                return new
                {
                    filename = info.Name,
                    size = info.Length,
                    created_at = info.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    download_url = "/download/" + info.Name
                };
            });

            return Task.FromResult<IActionResult>(Ok(new { total = files.Length, files = list }));
        }
    }
}

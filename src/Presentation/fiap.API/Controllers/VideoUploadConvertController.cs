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
        public VideoUploadConvertController(Serilog.ILogger logger, IStatusUploadService statusUploadService, IVideoUploadApplication videoUploadApplication)
        {
            _logger = logger;
            _statusUploadService = statusUploadService;
            _videoUploadApplication = videoUploadApplication;
        }

        [HttpPost("Upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(IFormFile video)
        {
            var idVideo = Guid.NewGuid().ToString();
            var videoUpload = new VideoUpload
            {
                Id = idVideo,
                NomeArquivoOrigem = video.FileName,
                NomeArquivoGerado = $"{idVideo}.zip",
                StatusUpload = StatusUpload.Enviado,
                UrlS3 = "construindo...",
                Usuario = new Usuario
                {
                    Nome = "Usuário Teste",
                    Email = "lmunarim@gmail.com"
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

                string[] frames = await _videoUploadApplication.ConverterVideoFrames(video, videoUpload);

                videoUpload.StatusUpload = StatusUpload.Finalizado;
                await _statusUploadService.EnviarStatusAsync(videoUpload);

                return Ok(new
                {
                    success = true,
                    message = $"Extracted {frames.Length} frames",
                    zip = videoUpload.UrlS3,
                    frameCount = frames.Length,
                    images = frames.Select(f => Path.GetFileName(f)).ToList()
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
        public Task<IActionResult> BaixarZip(string filename)
        {
            var path = Path.Combine(@"/app/outputs", filename);
            if (!System.IO.File.Exists(path))
                return Task.FromResult<IActionResult>(NotFound("File not found"));

            return Task.FromResult<IActionResult>(File(System.IO.File.OpenRead(path), "application/zip", filename));
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

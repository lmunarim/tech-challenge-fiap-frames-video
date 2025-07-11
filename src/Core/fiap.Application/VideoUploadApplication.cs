using FFMpegCore;
using fiap.Application.Interfaces;
using fiap.Domain.Entities;
using fiap.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace fiap.Application
{
    public class VideoUploadApplication :IVideoUploadApplication
    {
        private readonly Serilog.ILogger _logger;
        private readonly IUploadArquivoService _uploadArquivoService;
        private readonly IStatusUploadService _statusUploadService;
        public VideoUploadApplication(Serilog.ILogger logger, IUploadArquivoService uploadArquivoService, IStatusUploadService statusUploadService)
        {
            _logger = logger;
            _uploadArquivoService = uploadArquivoService;
            _statusUploadService = statusUploadService;
        }
        public async Task<string[]> ConverterVideoFrames(IFormFile video, VideoUpload videoUpload)
        {
            videoUpload.StatusUpload = StatusUpload.Processando;
            await _statusUploadService.EnviarStatusAsync(videoUpload);

            var videoFile = $@"/app/uploads/{videoUpload.Id}_{Path.GetFileName(video.FileName)}";

            using (var stream = System.IO.File.Create(videoFile))
                await video.CopyToAsync(stream);

            var tempDir = $@"/app/temporary/{videoUpload.Id}";

            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            await FFMpegArguments
                .FromFileInput(videoFile)
                .OutputToFile($@"{tempDir}/frame_%04d.png", overwrite: true, options =>
                                options.WithCustomArgument("-vf fps=1")) // 1 frame por segundo
                .ProcessAsynchronously();

            _logger.Information($" tempDir -->>> {tempDir}");

            string[] frames = Directory.GetFiles(tempDir, "*.png");
            var zipName = $"{videoUpload.Id}.zip";
            var zipPath = $@"/app/outputs/{zipName}";
            ZipFile.CreateFromDirectory(tempDir, zipPath);

            try
            {
                videoUpload.StatusUpload = StatusUpload.SalvandoS3;
                videoUpload.UrlS3 = "criando-endereco-S3";
                await _statusUploadService.EnviarStatusAsync(videoUpload);

                var urlS3 = await _uploadArquivoService.UploadAsync(zipPath);
                _logger.Information($"Upload ao S3 realizado com sucesso do arquivo {zipName}");

                videoUpload.StatusUpload = StatusUpload.ArquivoSalvoS3;
                videoUpload.UrlS3 = urlS3;
                await _statusUploadService.EnviarStatusAsync(videoUpload);
            }
            catch (Exception ex)
            {
                videoUpload.StatusUpload = StatusUpload.ErroProcessamento;
                await _statusUploadService.EnviarStatusAsync(videoUpload);

                _logger.Error(ex, $"Erro ao no upload do arquivo {zipPath}");
            }

            Directory.Delete(tempDir, true);
            System.IO.File.Delete(videoFile);

            return frames;
        }

    }
}

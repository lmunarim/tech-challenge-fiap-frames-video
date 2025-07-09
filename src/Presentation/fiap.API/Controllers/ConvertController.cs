using FFMpegCore;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

using StackExchange.Redis;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace fiap.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConvertController : ControllerBase
    {
        private readonly Serilog.ILogger _logger;
        public ConvertController(Serilog.ILogger logger)
        {
            _logger = logger;
        }

        [HttpPost("Upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(IFormFile video)
        {
            try
            {
                if (video == null || video.Length == 0)
                    return BadRequest(new { success = false, message = "No video uploaded" });

                var ext = Path.GetExtension(video.FileName).ToLower();
                var allowed = new[] { ".mp4", ".avi", ".mov", ".mkv", ".flv", ".wmv", ".webm" };
                if (!allowed.Contains(ext))
                    return BadRequest(new { success = false, message = "Unsupported format" });

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss"); // mudar guid
                var videoFile = $@"/app/uploads/{timestamp}_{Path.GetFileName(video.FileName)}";

                using (var stream = System.IO.File.Create(videoFile))
                    await video.CopyToAsync(stream);

                await SalvarRedisAsync(stream);

                var tempDir = $@"/app/temporary/{timestamp}";

                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);

                await FFMpegArguments
                    .FromFileInput(videoFile)
                    .OutputToFile($@"{tempDir}/frame_%04d.png", overwrite: true, options =>
                                    options.WithCustomArgument("-vf fps=1")) // 1 frame por segundo
                    .ProcessAsynchronously();

                _logger.Information($" tempDir -->>> {tempDir}");
                var frames = Directory.GetFiles(tempDir, "*.png");
                if (frames.Length == 0)
                    return BadRequest(new { success = false, message = "No frames extracted" });

                var zipName = $"frames_{timestamp}.zip";
                var zipPath = $@"/app/outputs/{zipName}";
                ZipFile.CreateFromDirectory(tempDir, zipPath);

                Directory.Delete(tempDir, true);
                System.IO.File.Delete(videoFile);

                return Ok(new
                {
                    success = true,
                    message = $"Extracted {frames.Length} frames",
                    zip = $"/download/{zipName}",
                    frameCount = frames.Length,
                    images = frames.Select(f => Path.GetFileName(f)).ToList()
                });
            }
            catch (Exception ex)
            {
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
        /// <summary>
        /// 
        /// </summary>
        public async Task SalvarRedisAsync(FileStream stream)
        {
            try
            {

            
            byte[] byteArray;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                byteArray = memoryStream.ToArray();
            }

            var newObj = new {

                nomeArquivo = Guid.NewGuid().ToString(),
                conteudo = byteArray
            };

            var options = new ConfigurationOptions
            {
                EndPoints = { "fiapfase5redis-xzjgcs.serverless.use1.cache.amazonaws.com:6379" },
                Ssl = true
            };

            var redis = ConnectionMultiplexer.Connect(options);
            var db = redis.GetDatabase();

            await db.StringSetAsync("mp4", JsonSerializer.Serialize(newObj));
            }
            catch (Exception ex)
            {
                _logger.Error(ex,$"Erro ao salvar no Redis - {ex.Message} - {ex.InnerException.Message}");
                throw;
            }
        }
    }
}

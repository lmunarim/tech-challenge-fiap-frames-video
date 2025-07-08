using Amazon;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO.Compression;

namespace fiap.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConvertController : ControllerBase
    {
        private readonly ILogger<ConvertController> _logger;
        public ConvertController(ILogger<ConvertController> logger)
        {
            _logger = logger;
        }

        [HttpPost("Upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(IFormFile video)
        {
            if (video == null || video.Length == 0)
                return BadRequest(new { success = false, message = "No video uploaded" });

            var ext = Path.GetExtension(video.FileName).ToLower();
            var allowed = new[] { ".mp4", ".avi", ".mov", ".mkv", ".flv", ".wmv", ".webm" };
            if (!allowed.Contains(ext))
                return BadRequest(new { success = false, message = "Unsupported format" });

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var videoFile = $@"..\uploads\{timestamp}_{Path.GetFileName(video.FileName)}";

            using (var stream = System.IO.File.Create(videoFile))
                await video.CopyToAsync(stream);

            var tempDir = $@"..\temporary\{timestamp}";

            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            var ffmpeg = new ProcessStartInfo
            {
                FileName = @"..\lib\ffmpeg\v4\ffmpeg",
                
                Arguments = $"-i \"{videoFile}\" -vf fps=1 -y \"{tempDir}/frame_%04d.png\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            var proc = Process.Start(ffmpeg);
            proc.WaitForExit();

            var frames = Directory.GetFiles(tempDir, "*.png");
            if (frames.Length == 0)
                return BadRequest(new { success = false, message = "No frames extracted" });

            var zipName = $"frames_{timestamp}.zip";
            var zipPath = $@"..\outputs\{zipName}";
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

        [HttpGet("BaixarZip")]
        public Task<IActionResult> BaixarZip(string filename)
        {
            var path = Path.Combine($@"..\outputs", filename);
            if (!System.IO.File.Exists(path))
                return Task.FromResult<IActionResult>(NotFound("File not found"));

            return Task.FromResult<IActionResult>(File(System.IO.File.OpenRead(path), "application/zip", filename));
        }

        [HttpGet("Status")]
        public Task<IActionResult> Status(string filename)
        {
            var files = Directory.GetFiles($@"..\outputs", "*.zip");
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

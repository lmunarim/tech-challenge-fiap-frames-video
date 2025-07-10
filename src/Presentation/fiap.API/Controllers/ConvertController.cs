using FFMpegCore;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

using StackExchange.Redis;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Collections;
using System.Security.Cryptography;
using fiap.API.DTO;
using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon;

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

                try
                {
                    byte[] byteArray = await System.IO.File.ReadAllBytesAsync(zipPath);
                    await EnviarFilaAsync(byteArray, zipName, new UsuarioDTO { Email = "teste@teste.com", Nome = "teste" });
                }
                catch(Exception ex)
                {
                    _logger.Error(ex, $"Erro ao enviar para a fila");
                }

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
        private async Task EnviarFilaAsync(byte[] byteArray, string nomeArquivo, UsuarioDTO usuario)
        {
            try
            {
                var newObj = new
                {

                    nomeArquivo,
                    conteudo = byteArray,
                    usuario
                };

                var sqsClient = new AmazonSQSClient(RegionEndpoint.USEast1);
                var sendRequest = new SendMessageRequest
                {
                    QueueUrl = "https://sqs.us-east-1.amazonaws.com/147997141255/tech-challenge-fiap-upload-imagens",
                    MessageBody = JsonSerializer.Serialize(newObj)
                };

                _ = await sqsClient.SendMessageAsync(sendRequest);

                /// _logger.Information($"Array de bytes do zip enviado para fila com sucesso {response}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Erro ao salvar na fila - {ex.Message} - {ex.InnerException.Message}");
                throw;
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //private async Task SalvarRedisAsync(byte[] byteArray)
        //{
        //    try
        //    {
        //    var newObj = new {

        //        nomeArquivo = Guid.NewGuid().ToString(),
        //        conteudo = byteArray
        //    };

        //        var config = new ConfigurationOptions
        //        {
        //            EndPoints = { "fiapfase5redis-xzjgcs.serverless.use1.cache.amazonaws.com:6379" },
        //            AbortOnConnectFail = false,
        //            Ssl = true,
        //            ConnectTimeout = 10000,
        //            SyncTimeout = 10000,
        //            KeepAlive = 180,
        //            ClientName = "fiapfase5redis"
        //        };

        //    var redis = ConnectionMultiplexer.Connect(config);
        //    var db = redis.GetDatabase();

        //    await db.StringSetAsync("mp4", JsonSerializer.Serialize(newObj));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex,$"Erro ao salvar no Redis - {ex.Message} - {ex.InnerException.Message}");
        //        throw;
        //    }
        //}
    }
}

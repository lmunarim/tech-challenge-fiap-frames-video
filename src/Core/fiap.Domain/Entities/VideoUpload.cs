using System;
using System.Text.Json.Serialization;

namespace fiap.Domain.Entities
{
    public class VideoUpload
    {
        /// <summary>
        /// Guid gerado na API
        /// </summary>
        public string Id { get; set; }
        public string NomeArquivoOrigem { get; set; }
        public string NomeArquivoGerado { get; set; }
        public string UrlS3 { get; set; }
        public Usuario Usuario { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StatusUpload StatusUpload { get; set; }
        [JsonIgnore]
        public DateTime DataUpload { get; set; }

    }
}

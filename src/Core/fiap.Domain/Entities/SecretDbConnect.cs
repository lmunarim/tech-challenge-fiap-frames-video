using System.Text.Json.Serialization;

namespace fiap.Domain.Entities
{
    public class SecretDbConnect
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
        [JsonPropertyName("engine")]
        public string Engine { get; set; }
        [JsonPropertyName("host")]
        public string Host { get; set; }
        [JsonPropertyName("port")]
        public string Port { get; set; }
        [JsonPropertyName("dbInstanceIdentifier")]
        public string DbInstanceIdentifier { get; set; }
    }
}

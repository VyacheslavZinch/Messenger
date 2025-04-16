using System.Text.Json.Serialization;

namespace APIInterfaces
{
    public record Authentication
    {
        [JsonPropertyName("login")]
        public string Login{ get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}

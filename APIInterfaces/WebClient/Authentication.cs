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

    public record AuthenticationResponse
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("userNickname")]
        public string UserNickName { get; set; }

        [JsonPropertyName("bearerToken")]
        public string Token { get; set; }

    }
}

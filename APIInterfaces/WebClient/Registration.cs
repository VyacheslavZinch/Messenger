using System.Text.Json.Serialization;

namespace APIInterfaces
{
    public record Registration
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("usernickname")]
        public string? UserNickname { get; set; }

        [JsonPropertyName("usermail")]
        public string UserEmail { get; set; }

        [JsonPropertyName("userphonenumber")]
        public string? UserPhoneNumber { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }
}

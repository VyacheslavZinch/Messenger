using System.Text.Json.Serialization;

namespace APIInterfaces
{
    public record RestoreAccess
    {
        [JsonPropertyName("mail")]
        public string Mail { get; set; }
    }
}

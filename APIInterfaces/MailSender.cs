using System.Text.Json.Serialization;

namespace APIInterfaces
{
    public struct MailSenderResult
    {
        [JsonPropertyName("result")]
        public bool Result { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("datetime")]
        public string MessageDateTime { get; set; }

    }

    public struct MessageServiceAPIError
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public struct IncomingRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }



}

using System.Text.Json.Serialization;

namespace APIInterfaces
{
    public struct MailSenderResult
    {
        [JsonPropertyName("result")]
        public bool result { get; set; }

        [JsonPropertyName("message")]
        public string message { get; set; }

        [JsonPropertyName("datetime")]
        public string message_date_time { get; set; }

    }

    public struct MessageServiceAPIError
    {
        [JsonPropertyName("message")]
        public string message { get; set; }
    }

    public struct IncomingRequest
    {
        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("email")]
        public string email { get; set; }
    }



}

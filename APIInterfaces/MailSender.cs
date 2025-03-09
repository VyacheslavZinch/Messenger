using System.Net;
using System.Text.Json.Serialization;

namespace APIInterfaces
{
    public record MailSenderResult
    {
        [JsonPropertyName("statuscode")]
        public HttpStatusCode StatusCode { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("type")]
        public string MailType { get; set; }

        [JsonPropertyName("datetime")]
        public DateTime MessageDateTime { get; set; }

        [JsonPropertyName("error")]
        public MessageServiceAPIError MessageServiceAPIError { get; set; }

    }

    public record MessageServiceAPIError
    {
        [JsonPropertyName("message")]
        public string Message { get; init; }
    }

    public record IncomingRequestRegistration
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

    }
    public record IncomingRequestRestoreAccess
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("newpassword")]
        public string NewPassword { get; set; }

    }
    public enum MailType
    {
        Registration,
        RestoreAccess
    }
}

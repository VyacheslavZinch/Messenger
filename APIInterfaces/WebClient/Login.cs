using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Net;

namespace APIInterfaces.WebClient
{
    public record LoginIncomingRequest
    {
        [JsonPropertyName("username")]
        public string UsernameMail { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public record LoginDataDb
    {

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("salt")]
        public string Salt { get; set; }

    }
    public record UserTokenRequest
    {
        [JsonPropertyName("userid")]
        public string UserId { get; set; }
    }

    public record UserTokenResponse
    {
        [JsonPropertyName("token")]
        public string UserToken { get; set; }
    }

    public record LoginResponse
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("token")]
        public string UserToken { get; set; }
    }

}

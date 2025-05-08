using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APIInterfaces.WebClient
{
    public class UserSearchResponse
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("userEmail")]
        public string UserEmail { get; set; }

        [JsonPropertyName("userNickname")]
        public string UserNickname { get; set; }
    }
}

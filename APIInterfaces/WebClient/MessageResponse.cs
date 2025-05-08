using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APIInterfaces.WebClient
{
    public class MessageResponse
    {
        [JsonPropertyName("chatMessage")]
        public string ChatMessage { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("sendDate")]
        public DateTime SendDate { get; set; }

        [JsonPropertyName("senderNickname")]
        public string SenderNickname { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APIInterfaces.WebClient
{
    public class ChatResponse
    {
        [JsonPropertyName("chatId")]
        public int ChatId { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("lastMessage")]
        public string LastMessage { get; set; }
    }
}

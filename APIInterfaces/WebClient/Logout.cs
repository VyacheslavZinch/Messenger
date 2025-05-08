using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APIInterfaces.WebClient
{
    public record Logout
    {
        [JsonPropertyName("userid")]
        public string UserId { get; set; }
    }
}

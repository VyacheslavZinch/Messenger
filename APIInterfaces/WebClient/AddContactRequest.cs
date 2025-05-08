using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APIInterfaces.WebClient
{
    public class AddContactRequest
    {
        [JsonPropertyName("contactId")]
        public string ContactId { get; set; }
    }
}

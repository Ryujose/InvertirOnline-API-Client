using IOLApiClient.Communication.Abstractions.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace IOLApiClient.Communication.Abstractions.Models
{
    public class MessageModel : IMessageModel
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}

using IOLApiClient.Communication.Abstractions.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace IOLApiClient.Communication.Abstractions.Models
{
    public class MessageOperationModel : IMessageOperationModel
    {
        [JsonPropertyName("numeroOperacion")]
        public int OperationNumber { get; set; }
    }
}

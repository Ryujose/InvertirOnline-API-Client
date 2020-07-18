using IOLApiClient.Operative.Abstractions.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace IOLApiClient.Operative.Abstractions.Models
{
    public class OperativeFCIModel : IOperativeFCIModel
    {
        [JsonPropertyName("simbolo")]
        public string Symbol { get; set; }
        [JsonPropertyName("monto")]
        public string Amount { get; set; }
        [JsonPropertyName("soloValidar")]
        public string OnlyValidate { get; set; }
    }
}

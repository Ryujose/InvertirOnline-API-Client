using IOLApiClient.Communication.Abstractions.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace IOLApiClient.Communication.Abstractions.Models
{
    public class TariffDataModel : ITariffDataModel
    {
        [JsonPropertyName("tipo")]
        public string Type { get; set; }
        [JsonPropertyName("neto")]
        public decimal Net { get; set; }
        [JsonPropertyName("iva")]
        public decimal VAT { get; set; }
    }
}

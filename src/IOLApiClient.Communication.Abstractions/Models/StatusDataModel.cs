using IOLApiClient.Communication.Abstractions.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace IOLApiClient.Communication.Abstractions.Models
{
    public class StatusDataModel : IStatusDataModel
    {
        [JsonPropertyName("detalle")]
        public string Detail { get; set; }
        [JsonPropertyName("fecha")]
        public string Date { get; set; }
    }
}

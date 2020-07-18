using IOLApiClient.Operative.Abstractions.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace IOLApiClient.Operative.Abstractions.Models
{
    public class OperativeModel : IOperativeModel
    {
        [JsonPropertyName("Validez")]
        public DateTime ValidityDate { get; set; }
        [JsonPropertyName("Mercado")]
        public string Market { get; set; }
        [JsonPropertyName("Plazo")]
        public string Term { get; set; }
        [JsonPropertyName("Cantidad")]
        public decimal Quantity { get; set; }
        [JsonPropertyName("Precio")]
        public decimal Price { get; set; }
        [JsonPropertyName("Simbolo")]
        public string Symbol { get; set; }
    }
}

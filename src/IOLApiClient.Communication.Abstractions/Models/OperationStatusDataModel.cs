using IOLApiClient.Communication.Abstractions.Models.Interfaces;
using System.Text.Json.Serialization;

namespace IOLApiClient.Communication.Abstractions.Models
{
    public class OperationStatusDataModel : IOperationStatusModel
    {
        [JsonPropertyName("fecha")]
        public string Date { get; set; }
        [JsonPropertyName("cantidad")]
        public decimal Quantity { get; set; }
        [JsonPropertyName("precio")]
        public decimal Price { get; set; }
    }
}

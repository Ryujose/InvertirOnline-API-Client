using IOLApiClient.Communication.Abstractions.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace IOLApiClient.Communication.Abstractions.Models
{
    public class TransactionDataModel : ITransactionDataModel
    {
        [JsonPropertyName("numero")]
        public int Number { get; set; }
        [JsonPropertyName("mercado")]
        public string Market { get; set; }
        [JsonPropertyName("simbolo")]
        public string Symbol { get; set; }
        [JsonPropertyName("moneda")]
        public string Coin { get; set; }
        [JsonPropertyName("tipo")]
        public string Type { get; set; }
        [JsonPropertyName("fechaAlta")]
        public string CreationDate { get; set; }
        [JsonPropertyName("validez")]
        public string Validity { get; set; }
        [JsonPropertyName("fechaOperado")]
        public string OperationDate { get; set; }
        [JsonPropertyName("estadoActual")]
        public string CurrentStatus { get; set; }
        [JsonPropertyName("precio")]
        public decimal? Price { get; set; }
        [JsonPropertyName("cantidad")]
        public decimal Quantity { get; set; }
        [JsonPropertyName("monto")]
        public decimal Amount { get; set; }
        [JsonPropertyName("modalidad")]
        public string Modality { get; set; }
        [JsonPropertyName("estados")]
        public IEnumerable<StatusDataModel> Statuses { get; set; }
        [JsonPropertyName("aranceles")]
        public IEnumerable<TariffDataModel> Tarrifs { get; set; }
        [JsonPropertyName("operaciones")]
        public IEnumerable<OperationStatusDataModel> OperationStatusDataModel { get; set; }
    }
}

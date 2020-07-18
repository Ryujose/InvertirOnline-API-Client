using IOLApiClient.Communication.Abstractions.Models.Interfaces;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IOLApiClient.Communication.Abstractions.Models
{
    public class ResponseModel : IResponseModel
    {
        [JsonPropertyName("ok")]
        public bool IsOk { get; set; }
        [JsonPropertyName("messages")]
        public IEnumerable<MessageModel> Messages { get; set; }
    }
}

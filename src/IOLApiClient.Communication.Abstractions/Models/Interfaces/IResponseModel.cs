using System.Collections.Generic;

namespace IOLApiClient.Communication.Abstractions.Models.Interfaces
{
    public interface IResponseModel
    {
        bool IsOk { get; set; }
        IEnumerable<MessageModel> Messages { get; set; }
    }
}

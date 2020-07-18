using System;
using System.Collections.Generic;
using System.Text;

namespace IOLApiClient.Communication.Abstractions.Models.Interfaces
{
    public interface ITransactionDataModel
    {
        int Number { get; set; }
        string Market { get; set; }
        string Symbol { get; set; }
        string Coin { get; set; }
        string Type { get; set; }
        string CreationDate { get; set; }
        string Validity { get; set; }
        string OperationDate { get; set; }
        string CurrentStatus { get; set; }
        decimal? Price { get; set; }
        decimal Quantity { get; set; }
        decimal Amount { get; set; }
        string Modality { get; set; }
        IEnumerable<StatusDataModel> Statuses { get; set;}
        IEnumerable<TariffDataModel> Tarrifs { get; set; }
        IEnumerable<OperationStatusDataModel> OperationStatusDataModel { get; set; }

    }
}

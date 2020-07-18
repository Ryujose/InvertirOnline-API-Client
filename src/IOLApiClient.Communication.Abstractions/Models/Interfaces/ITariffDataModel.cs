using System;
using System.Collections.Generic;
using System.Text;

namespace IOLApiClient.Communication.Abstractions.Models.Interfaces
{
    public interface ITariffDataModel
    {
        string Type { get; set; }
        decimal Net { get; set; }
        decimal VAT { get; set; }
    }
}

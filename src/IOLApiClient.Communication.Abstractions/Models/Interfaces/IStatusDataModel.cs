using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace IOLApiClient.Communication.Abstractions.Models.Interfaces
{
    public interface IStatusDataModel
    {
        string Detail { get; set; }
        string Date { get; set; }
    }
}

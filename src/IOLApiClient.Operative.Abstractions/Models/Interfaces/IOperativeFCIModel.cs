using System;
using System.Collections.Generic;
using System.Text;

namespace IOLApiClient.Operative.Abstractions.Models.Interfaces
{
    public interface IOperativeFCIModel
    {
        string Symbol { get; set; }
        string Amount { get; set; }
        string OnlyValidate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace IOLApiClient.Operative.Abstractions.Models.Interfaces
{
    public interface IOperativeModel
    {
        DateTime ValidityDate { get; set; }
        string Market { get; set; }
        string Term { get; set; }
        decimal Quantity { get; set; }
        decimal Price { get; set; }
        string Symbol { get; set; }
    }
}

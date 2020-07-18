using System;
using System.Collections.Generic;
using System.Text;

namespace IOLApiClient.Communication.Abstractions.Models.Interfaces
{
    public interface IMessageModel
    {
        string Title { get; set; }
        string Description { get; set; }
    }
}

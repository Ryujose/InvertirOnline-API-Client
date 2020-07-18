using IOLApiClient.Communication.Abstractions.Models;
using IOLApiClient.Communication.Abstractions.Models.Interfaces;
using IOLApiClient.Operative.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOLApiClient.Operative.Sell.Repository.Abstractions.Interfaces.V2
{
    public interface ISellRepository
    {
        Task<IResponseModel> Sell(OperativeModel operativeModel);
    }
}

using IOLApiClient.Communication.Abstractions.Models.Interfaces;
using IOLApiClient.Operative.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOLApiClient.Operative.Retrieval.Repository.Abstractions.Interfaces.V2
{
    public interface IRetrievalRepository
    {
        Task<IResponseModel> RetrieveFCI(OperativeFCIModel operativeFCIModel);
    }
}

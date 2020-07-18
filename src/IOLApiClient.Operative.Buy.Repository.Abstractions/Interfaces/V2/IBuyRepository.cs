using IOLApiClient.Communication.Abstractions.Models;
using IOLApiClient.Communication.Abstractions.Models.Interfaces;
using IOLApiClient.Operative.Abstractions.Models;
using System.Threading.Tasks;

namespace IOLApiClient.Operative.Buy.Repository.Abstractions.Interfaces.V2
{
    public interface IBuyRepository
    {
        Task<IResponseModel> Buy(OperativeModel operativeModel);
    }
}

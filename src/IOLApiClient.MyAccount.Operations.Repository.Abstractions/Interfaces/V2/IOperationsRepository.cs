using IOLApiClient.Communication.Abstractions.Models.Interfaces;
using System.Threading.Tasks;

namespace IOLApiClient.MyAccount.Operations.Repository.Abstractions.Interfaces.V2
{
    public interface IOperationsRepository
    {
        Task<ITransactionDataModel> GetTransaction(int transactionId);
        Task<IResponseModel> DeleteTransaction(int transactionId);
    }
}

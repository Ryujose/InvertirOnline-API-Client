using System;
using System.Threading.Tasks;

namespace IOLApiClient.Auth.Repository.Abstractions.Interfaces
{
    public interface ILoginRepository
    {
        Task Login();
    }
}

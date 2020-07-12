using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOLApiClient.Auth.Repository.Abstractions.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task RefreshToken();
    }
}

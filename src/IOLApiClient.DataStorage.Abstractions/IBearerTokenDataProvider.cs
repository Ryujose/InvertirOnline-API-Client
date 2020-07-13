using IOLApiClient.Auth.Repository.Abstractions.Models;

namespace IOLApiClient.DataStorage.Abstractions
{
    public interface IBearerTokenDataProvider
    {
        LoginResponseModel LoginResponseModel { get; set; }

        void SetLoginResponseModel(LoginResponseModel loginResponseModel);
    }
}

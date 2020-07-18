using IOLApiClient.Auth.Repository.Abstractions.Configuration;
using IOLApiClient.Communication.Abstractions.Models;
using IOLApiClient.Communication.Abstractions.Models.Interfaces;
using IOLApiClient.DataStorage.Abstractions;
using IOLApiClient.Operative.Abstractions.Models;
using IOLApiClient.Operative.Sell.Repository.Abstractions.Interfaces.V2;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IOLApiClient.Operative.Sell.Repository.Repositories.V2
{
    public class SellRepository : ISellRepository
    {
        static string _classSellRepositoryMessageDiagnose = $"Class: {nameof(SellRepository)}";
        static string _methodBuyMessageDiagnose = $"Method: {nameof(Sell)}";
        static string _methodBuildDefaultHeadersMessageDiagnose = $"Method: {nameof(BuildDefaultHeaders)}";
        static string _methodBuildSellURLMessageDiagnose = $"Method: {nameof(BuildSellURL)}";

        private readonly IBearerTokenDataProvider _bearerTokenDataProvider;
        private readonly ILogger _logger;
        private readonly ILoginRepositorySettings _loginRepositorySettings;

        public SellRepository(
            IBearerTokenDataProvider bearerTokenDataProvider,
            ILogger logger,
            ILoginRepositorySettings loginRepositorySettings)
        {
            _bearerTokenDataProvider = bearerTokenDataProvider;
            _logger = logger;
            _loginRepositorySettings = loginRepositorySettings;
        }

        public async Task<IResponseModel> Sell(OperativeModel operativeModel)
        {
            using (var client = new HttpClient())
            {
                BuildDefaultHeaders(client);

                _logger.Information($"{_classSellRepositoryMessageDiagnose} {_methodBuyMessageDiagnose}, Initializing sell...");

                if (_bearerTokenDataProvider.LoginResponseModel == null)
                    throw new InvalidOperationException($"{nameof(IBearerTokenDataProvider.LoginResponseModel)} is null, can't sell.");

                if (string.IsNullOrEmpty(_bearerTokenDataProvider.LoginResponseModel.RefreshToken))
                    throw new InvalidOperationException($"{nameof(IBearerTokenDataProvider.LoginResponseModel.RefreshToken)} is null or empty, can't sell.");


                using (var content = new StringContent(JsonSerializer.Serialize(operativeModel, typeof(OperativeModel)), Encoding.UTF8, "application/json"))
                {
                    var result = await client.PostAsync(BuildSellURL(), content);

                    _logger.Information($"{_classSellRepositoryMessageDiagnose} {_methodBuyMessageDiagnose}, {(result != null ? $"Sell result code: {result.StatusCode}" : "Sell is null")}");

                    result.EnsureSuccessStatusCode();

                    IResponseModel responseModel = null;

                    switch (result.StatusCode)
                    {
                        case HttpStatusCode.Created:
                            MessageOperationModel messageOperation = await JsonSerializer.DeserializeAsync<MessageOperationModel>(await result.Content.ReadAsStreamAsync());

                            responseModel = new ResponseModel
                            {
                                IsOk = true,
                                Messages = new List<MessageModel>
                                {
                                    new MessageModel
                                    {
                                        Title = "TransactionID",
                                        Description = messageOperation.OperationNumber.ToString()
                                    }
                                }
                            };
                            break;
                        case HttpStatusCode.Accepted:
                        case HttpStatusCode.NotFound:
                        case HttpStatusCode.Unauthorized:
                            IEnumerable<MessageModel> message = await JsonSerializer.DeserializeAsync<IEnumerable<MessageModel>>(await result.Content.ReadAsStreamAsync());

                            responseModel = new ResponseModel
                            {
                                Messages = message
                            };
                            break;
                        default:
                            throw new InvalidOperationException($"Result: {result.StatusCode} isn't expected to resolve {nameof(ResponseModel)} object");
                    }

                    return responseModel;
                }
            }
        }

        private string BuildSellURL()
        {
            var buyURL = $"{_loginRepositorySettings.BaseUrl}/api/v2/operar/Vender";

            if (_logger.IsEnabled(LogEventLevel.Debug))
            {
                _logger.Debug($"{_classSellRepositoryMessageDiagnose} {_methodBuildSellURLMessageDiagnose}, BuildSellURL: {buyURL}");
            }

            return buyURL;
        }

        private void BuildDefaultHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerTokenDataProvider.LoginResponseModel.AccesToken);
            client.DefaultRequestHeaders.Add("User-Agent", ".NET-Core-3.1-client");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

            if (_logger.IsEnabled(LogEventLevel.Debug))
            {
                foreach (var defaultRequestHeader in client.DefaultRequestHeaders)
                {
                    IEnumerable<string> values = defaultRequestHeader.Value;

                    foreach (var value in values)
                    {
                        _logger.Debug($"{_classSellRepositoryMessageDiagnose} {_methodBuildDefaultHeadersMessageDiagnose}, default header assigned key: {defaultRequestHeader.Key}, value: {value}");
                    }
                }
            }
        }
    }
}

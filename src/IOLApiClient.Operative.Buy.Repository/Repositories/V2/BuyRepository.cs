using IOLApiClient.Auth.Repository.Abstractions.Configuration;
using IOLApiClient.Communication.Abstractions.Models;
using IOLApiClient.Communication.Abstractions.Models.Interfaces;
using IOLApiClient.DataStorage.Abstractions;
using IOLApiClient.Operative.Abstractions.Models;
using IOLApiClient.Operative.Buy.Repository.Abstractions.Interfaces.V2;
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

namespace IOLApiClient.Operative.Buy.Repository.Repositories.V2
{
    public class BuyRepository : IBuyRepository
    {
        static string _classBuyRepositoryMessageDiagnose = $"Class: {nameof(BuyRepository)}";
        static string _methodBuyMessageDiagnose = $"Method: {nameof(Buy)}";
        static string _methodBuildDefaultHeadersMessageDiagnose = $"Method: {nameof(BuildDefaultHeaders)}";
        static string _methodBuildBuyURLMessageDiagnose = $"Method: {nameof(BuildBuyURL)}";

        private readonly IBearerTokenDataProvider _bearerTokenDataProvider;
        private readonly ILogger _logger;
        private readonly ILoginRepositorySettings _loginRepositorySettings;

        public BuyRepository(
            IBearerTokenDataProvider bearerTokenDataProvider,
            ILogger logger,
            ILoginRepositorySettings loginRepositorySettings)
        {
            _bearerTokenDataProvider = bearerTokenDataProvider;
            _logger = logger;
            _loginRepositorySettings = loginRepositorySettings;
        }

        public async Task<IResponseModel> Buy(OperativeModel operativeModel)
        {
            using(var client = new HttpClient())
            {
                BuildDefaultHeaders(client);

                _logger.Information($"{_classBuyRepositoryMessageDiagnose} {_methodBuyMessageDiagnose}, Initializing buy...");

                if (_bearerTokenDataProvider.LoginResponseModel == null)
                    throw new InvalidOperationException($"{nameof(IBearerTokenDataProvider.LoginResponseModel)} is null, can't buy.");

                if (string.IsNullOrEmpty(_bearerTokenDataProvider.LoginResponseModel.RefreshToken))
                    throw new InvalidOperationException($"{nameof(IBearerTokenDataProvider.LoginResponseModel.RefreshToken)} is null or empty, can't buy.");


                using (var content = new StringContent(JsonSerializer.Serialize(operativeModel, typeof(OperativeModel)), Encoding.UTF8, "application/json"))
                {
                    var result = await client.PostAsync(BuildBuyURL(), content);

                    _logger.Information($"{_classBuyRepositoryMessageDiagnose} {_methodBuyMessageDiagnose}, {(result != null ? $"Buy result code: {result.StatusCode}" : "Buy is null")}");

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
                            responseModel = await JsonSerializer.DeserializeAsync<ResponseModel>(await result.Content.ReadAsStreamAsync());
                            break;
                        default:
                            throw new InvalidOperationException($"Result: {result.StatusCode} isn't expected to resolve {nameof(ResponseModel)} object");
                    }

                    return responseModel;
                }
            }
        }

        private string BuildBuyURL()
        {
            var buyURL = $"{_loginRepositorySettings.BaseUrl}/api/v2/operar/Comprar";

            if (_logger.IsEnabled(LogEventLevel.Debug))
            {
                _logger.Debug($"{_classBuyRepositoryMessageDiagnose} {_methodBuildBuyURLMessageDiagnose}, BuildBuyURL: {buyURL}");
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
                        _logger.Debug($"{_classBuyRepositoryMessageDiagnose} {_methodBuildDefaultHeadersMessageDiagnose}, default header assigned key: {defaultRequestHeader.Key}, value: {value}");
                    }
                }
            }
        }
    }
}

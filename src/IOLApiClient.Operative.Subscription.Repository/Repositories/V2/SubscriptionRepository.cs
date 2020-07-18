using IOLApiClient.Auth.Repository.Abstractions.Configuration;
using IOLApiClient.Communication.Abstractions.Models;
using IOLApiClient.Communication.Abstractions.Models.Interfaces;
using IOLApiClient.DataStorage.Abstractions;
using IOLApiClient.Operative.Abstractions.Models;
using IOLApiClient.Operative.Subscription.Repository.Abstractions.Interfaces.V2;
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

namespace IOLApiClient.Operative.Subscription.Repository.Repositories.V2
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        static string _classSubscriptionRepositoryMessageDiagnose = $"Class: {nameof(SubscriptionRepository)}";
        static string _methodSubscribeFCIMessageDiagnose = $"Method: {nameof(SubscribeFCI)}";
        static string _methodBuildDefaultHeadersMessageDiagnose = $"Method: {nameof(BuildDefaultHeaders)}";
        static string _methodBuildSubscriptionURLMessageDiagnose = $"Method: {nameof(BuildSubscriptionURL)}";

        private readonly IBearerTokenDataProvider _bearerTokenDataProvider;
        private readonly ILogger _logger;
        private readonly ILoginRepositorySettings _loginRepositorySettings;

        public SubscriptionRepository(
            IBearerTokenDataProvider bearerTokenDataProvider,
            ILogger logger,
            ILoginRepositorySettings loginRepositorySettings)
        {
            _bearerTokenDataProvider = bearerTokenDataProvider;
            _logger = logger;
            _loginRepositorySettings = loginRepositorySettings;
        }

        public async Task<IResponseModel> SubscribeFCI(OperativeFCIModel operativeFCIModel)
        {
            using (var client = new HttpClient())
            {
                BuildDefaultHeaders(client);

                _logger.Information($"{_classSubscriptionRepositoryMessageDiagnose} {_methodSubscribeFCIMessageDiagnose}, Initializing subscription...");

                if (_bearerTokenDataProvider.LoginResponseModel == null)
                    throw new InvalidOperationException($"{nameof(IBearerTokenDataProvider.LoginResponseModel)} is null, can't subscribe.");

                if (string.IsNullOrEmpty(_bearerTokenDataProvider.LoginResponseModel.RefreshToken))
                    throw new InvalidOperationException($"{nameof(IBearerTokenDataProvider.LoginResponseModel.RefreshToken)} is null or empty, can't subscribe.");


                using (var content = new StringContent(JsonSerializer.Serialize(operativeFCIModel, typeof(OperativeFCIModel)), Encoding.UTF8, "application/json"))
                {
                    var result = await client.PostAsync(BuildSubscriptionURL(), content);

                    _logger.Information($"{_classSubscriptionRepositoryMessageDiagnose} {_methodSubscribeFCIMessageDiagnose}, {(result != null ? $"Subscribe result code: {result.StatusCode}" : "Subscribe is null")}");

                    result.EnsureSuccessStatusCode();

                    IResponseModel responseModel = null;

                    switch (result.StatusCode)
                    {
                        case HttpStatusCode.Accepted:
                        case HttpStatusCode.OK:
                            responseModel = await JsonSerializer.DeserializeAsync<ResponseModel>(await result.Content.ReadAsStreamAsync());
                            break;
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
                        default:
                            throw new InvalidOperationException($"Result: {result.StatusCode} isn't expected to resolve {nameof(ResponseModel)} object");
                    }

                    return responseModel;
                }
            }
        }

        private string BuildSubscriptionURL()
        {
            var subscriptionURL = $"{_loginRepositorySettings.BaseUrl}/api/v2/operar/Suscripcion/fci";

            if (_logger.IsEnabled(LogEventLevel.Debug))
            {
                _logger.Debug($"{_classSubscriptionRepositoryMessageDiagnose} {_methodBuildSubscriptionURLMessageDiagnose}, subscriptionURL: {subscriptionURL}");
            }

            return subscriptionURL;
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
                        _logger.Debug($"{_classSubscriptionRepositoryMessageDiagnose} {_methodBuildDefaultHeadersMessageDiagnose}, default header assigned key: {defaultRequestHeader.Key}, value: {value}");
                    }
                }
            }
        }
    }
}

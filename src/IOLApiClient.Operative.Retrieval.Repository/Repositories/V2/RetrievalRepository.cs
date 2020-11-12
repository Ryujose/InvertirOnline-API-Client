using IOLApiClient.Auth.Repository.Abstractions.Configuration;
using IOLApiClient.Communication.Abstractions.Models;
using IOLApiClient.Communication.Abstractions.Models.Interfaces;
using IOLApiClient.DataStorage.Abstractions;
using IOLApiClient.Operative.Abstractions.Models;
using IOLApiClient.Operative.Retrieval.Repository.Abstractions.Interfaces.V2;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IOLApiClient.Operative.Retrieval.Repository.Repositories.V2
{
    public class RetrievalRepository : IRetrievalRepository
    {
        static string _classRetrievalRepositoryMessageDiagnose = $"Class: {nameof(RetrievalRepository)}";
        static string _methodRetrieveFCIMessageDiagnose = $"Method: {nameof(RetrieveFCI)}";
        static string _methodBuildDefaultHeadersMessageDiagnose = $"Method: {nameof(BuildDefaultHeaders)}";
        static string _methodBuildRetrieveURLMessageDiagnose = $"Method: {nameof(BuildRetrieveURL)}";

        private readonly IBearerTokenDataProvider _bearerTokenDataProvider;
        private readonly ILogger<RetrievalRepository> _logger;
        private readonly ILoginRepositorySettings _loginRepositorySettings;

        public RetrievalRepository(
            IBearerTokenDataProvider bearerTokenDataProvider,
            ILogger<RetrievalRepository> logger,
            ILoginRepositorySettings loginRepositorySettings)
        {
            _bearerTokenDataProvider = bearerTokenDataProvider;
            _logger = logger;
            _loginRepositorySettings = loginRepositorySettings;
        }

        public async Task<IResponseModel> RetrieveFCI(OperativeFCIModel operativeFCIModel)
        {
            using (var client = new HttpClient())
            {
                BuildDefaultHeaders(client);

                _logger.LogInformation($"{_classRetrievalRepositoryMessageDiagnose} {_methodRetrieveFCIMessageDiagnose}, Initializing retrieval...");

                if (_bearerTokenDataProvider.LoginResponseModel == null)
                    throw new InvalidOperationException($"{nameof(IBearerTokenDataProvider.LoginResponseModel)} is null, can't retrieve.");

                if (string.IsNullOrEmpty(_bearerTokenDataProvider.LoginResponseModel.RefreshToken))
                    throw new InvalidOperationException($"{nameof(IBearerTokenDataProvider.LoginResponseModel.RefreshToken)} is null or empty, can't retrieve.");


                using (var content = new StringContent(JsonSerializer.Serialize(operativeFCIModel, typeof(OperativeFCIModel)), Encoding.UTF8, "application/json"))
                {
                    var result = await client.PostAsync(BuildRetrieveURL(), content);

                    _logger.LogInformation($"{_classRetrievalRepositoryMessageDiagnose} {_methodRetrieveFCIMessageDiagnose}, {(result != null ? $"Retrieve result code: {result.StatusCode}" : "Retrieve is null")}");

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

        private string BuildRetrieveURL()
        {
            var retrieveURL = $"{_loginRepositorySettings.BaseUrl}/api/v2/operar/Rescate/fci";

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"{_classRetrievalRepositoryMessageDiagnose} {_methodBuildRetrieveURLMessageDiagnose}, retrieveURL: {retrieveURL}");
            }

            return retrieveURL;
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

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                foreach (var defaultRequestHeader in client.DefaultRequestHeaders)
                {
                    IEnumerable<string> values = defaultRequestHeader.Value;

                    foreach (var value in values)
                    {
                        _logger.LogDebug($"{_classRetrievalRepositoryMessageDiagnose} {_methodBuildDefaultHeadersMessageDiagnose}, default header assigned key: {defaultRequestHeader.Key}, value: {value}");
                    }
                }
            }
        }
    }
}

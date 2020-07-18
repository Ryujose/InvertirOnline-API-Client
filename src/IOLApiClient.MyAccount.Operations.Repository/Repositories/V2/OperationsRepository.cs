using IOLApiClient.Auth.Repository.Abstractions.Configuration;
using IOLApiClient.Communication.Abstractions.Models;
using IOLApiClient.Communication.Abstractions.Models.Interfaces;
using IOLApiClient.DataStorage.Abstractions;
using IOLApiClient.MyAccount.Operations.Repository.Abstractions.Interfaces.V2;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace IOLApiClient.MyAccount.Operations.Repository.Repositories.V2
{
    public class OperationsRepository : IOperationsRepository
    {
        static string _classOperationsRepositoryMessageDiagnose = $"Class: {nameof(OperationsRepository)}";
        static string _methodDeleteTransactionMessageDiagnose = $"Method: {nameof(DeleteTransaction)}";
        static string _methodBuildDefaultHeadersMessageDiagnose = $"Method: {nameof(BuildDefaultHeaders)}";
        static string _methodOperationsDeleteTransactionURLMessageDiagnose = $"Method: {nameof(BuildOperationsDeleteTransactionURL)}";
        static string _methodBuildObtainOperationTransactionURLURLMessageDiagnose = $"Method: {nameof(BuildGetTransactionURL)}";

        private readonly IBearerTokenDataProvider _bearerTokenDataProvider;
        private readonly ILogger _logger;
        private readonly ILoginRepositorySettings _loginRepositorySettings;

        public OperationsRepository(
            IBearerTokenDataProvider bearerTokenDataProvider,
            ILogger logger,
            ILoginRepositorySettings loginRepositorySettings)
        {
            _bearerTokenDataProvider = bearerTokenDataProvider;
            _logger = logger;
            _loginRepositorySettings = loginRepositorySettings;
        }

        public async Task<ITransactionDataModel> GetTransaction(int transactionId)
        {
            using (var client = new HttpClient())
            {
                BuildDefaultHeaders(client);

                _logger.Information($"{_classOperationsRepositoryMessageDiagnose} {_methodDeleteTransactionMessageDiagnose}, Initializing GetTransaction...");

                if (_bearerTokenDataProvider.LoginResponseModel == null)
                    throw new InvalidOperationException($"{nameof(IBearerTokenDataProvider.LoginResponseModel)} is null, can't get transaction.");

                if (string.IsNullOrEmpty(_bearerTokenDataProvider.LoginResponseModel.RefreshToken))
                    throw new InvalidOperationException($"{nameof(IBearerTokenDataProvider.LoginResponseModel.RefreshToken)} is null or empty, can't get transaction.");

                using (var content = new StringContent(JsonSerializer.Serialize(new Dictionary<string, int> { { "numero", transactionId } })))
                {
                    var result = await client.GetAsync(BuildGetTransactionURL(transactionId));

                    _logger.Information($"{_classOperationsRepositoryMessageDiagnose} {_methodDeleteTransactionMessageDiagnose}, {(result != null ? $"get transaction result code: {result.StatusCode}" : "get transaction is null")}");

                    result.EnsureSuccessStatusCode();

                    ITransactionDataModel transactionDataModel = null;

                    switch (result.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            transactionDataModel = await JsonSerializer.DeserializeAsync<TransactionDataModel>(await result.Content.ReadAsStreamAsync());
                            break;
                        case HttpStatusCode.NotFound:
                            break;
                        case HttpStatusCode.Unauthorized:
                            break;
                        default:
                            throw new InvalidOperationException($"Result: {result.StatusCode} isn't expected to resolve {nameof(TransactionDataModel)} object");
                    }

                    return transactionDataModel;
                }
            }
        }

        public async Task<IResponseModel> DeleteTransaction(int transactionId)
        {
            using (var client = new HttpClient())
            {
                BuildDefaultHeaders(client);

                _logger.Information($"{_classOperationsRepositoryMessageDiagnose} {_methodDeleteTransactionMessageDiagnose}, Initializing DeleteTransaction...");

                if (_bearerTokenDataProvider.LoginResponseModel == null)
                    throw new InvalidOperationException($"{nameof(IBearerTokenDataProvider.LoginResponseModel)} is null, can't delete transaction.");

                if (string.IsNullOrEmpty(_bearerTokenDataProvider.LoginResponseModel.RefreshToken))
                    throw new InvalidOperationException($"{nameof(IBearerTokenDataProvider.LoginResponseModel.RefreshToken)} is null or empty, can't delete transaction.");

                var result = await client.DeleteAsync(BuildOperationsDeleteTransactionURL(transactionId));

                _logger.Information($"{_classOperationsRepositoryMessageDiagnose} {_methodDeleteTransactionMessageDiagnose}, {(result != null ? $"delete transaction result code: {result.StatusCode}" : "delete transaction is null")}");

                result.EnsureSuccessStatusCode();

                IResponseModel responseModel = null;

                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                        responseModel = await JsonSerializer.DeserializeAsync<ResponseModel>(await result.Content.ReadAsStreamAsync());
                        break;
                    default:
                        throw new InvalidOperationException($"Result: {result.StatusCode} isn't expected to resolve {nameof(ResponseModel)} object");
                }

                return responseModel;
            }
        }

        private string BuildOperationsDeleteTransactionURL(int transactionID)
        {
            var BuildOperationsDeleteURL = $"{_loginRepositorySettings.BaseUrl}/api/v2/operaciones/{transactionID}";

            if (_logger.IsEnabled(LogEventLevel.Debug))
            {
                _logger.Debug($"{_classOperationsRepositoryMessageDiagnose} {_methodOperationsDeleteTransactionURLMessageDiagnose}, BuildOperationsDeleteURL: {BuildOperationsDeleteURL}");
            }

            return BuildOperationsDeleteURL;
        }

        private string BuildGetTransactionURL(int transactionID)
        {
            var BuildGetTransactionURL = $"{_loginRepositorySettings.BaseUrl}/api/v2/operaciones/{transactionID}";

            if (_logger.IsEnabled(LogEventLevel.Debug))
            {
                _logger.Debug($"{_classOperationsRepositoryMessageDiagnose} {_methodBuildObtainOperationTransactionURLURLMessageDiagnose}, BuildGetTransactionURL: {BuildGetTransactionURL}");
            }

            return BuildGetTransactionURL;
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
                        _logger.Debug($"{_classOperationsRepositoryMessageDiagnose} {_methodBuildDefaultHeadersMessageDiagnose}, default header assigned key: {defaultRequestHeader.Key}, value: {value}");
                    }
                }
            }
        }
    }
}

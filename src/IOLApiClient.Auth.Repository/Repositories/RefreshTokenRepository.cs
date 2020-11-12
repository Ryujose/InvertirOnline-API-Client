using IOLApiClient.Auth.Repository.Abstractions.Configuration;
using IOLApiClient.Auth.Repository.Abstractions.Interfaces;
using IOLApiClient.Auth.Repository.Abstractions.Models;
using IOLApiClient.DataStorage.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace IOLApiClient.Auth.Repository.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        static string _classRefreshTokenMessageDiagnose = $"Class: {nameof(RefreshTokenRepository)}";
        static string _methodRefreshTokenMessageDiagnose = $"Method: {nameof(RefreshToken)}";
        static string _methodBuildTokenURLMessageDiagnose = $"Method: {nameof(BuildTokenURL)}";
        static string _methodLogLoginRespondeModelMessageDiagnose = $"Method: {nameof(LogLoginRespondeModel)}";
        static string _methodBuildDefaultHeadersMessageDiagnose = $"Method: {nameof(BuildDefaultHeaders)}";
        static string _methodRefreshTokenPostParametersMessageDiagnose = $"Method: {nameof(BuildRefreshTokenPostParameters)}";

        const string _GRANT_TYPE_REFRESH_TOKEN_POST_VALUE = "refresh_token";
        const string _GRANT_TYPE_POST_KEY = "grant_type";

        private readonly ILoginRepositorySettings _loginRepositorySettings;
        private readonly IBearerTokenDataProvider _bearerTokenData;
        private readonly ILogger<RefreshTokenRepository> _logger;

        public RefreshTokenRepository(
            ILoginRepositorySettings loginRepositorySettings,
            IBearerTokenDataProvider bearerTokenData,
            ILogger<RefreshTokenRepository> logger
            )
        {
            _loginRepositorySettings = loginRepositorySettings;
            _bearerTokenData = bearerTokenData;
            _logger = logger;
        }

        /// <summary>
        /// RefreshToken method that will refresh token data stored in IBearerTokenDataProvider
        /// </summary>
        public async Task RefreshToken()
        {
            _logger.LogInformation($"{_classRefreshTokenMessageDiagnose} {_methodRefreshTokenMessageDiagnose}, Initializing refresh token...");

            if (_bearerTokenData.LoginResponseModel == null)
                throw new InvalidOperationException($"{nameof(IBearerTokenDataProvider.LoginResponseModel)} is null, can't refresh token.");

            if (string.IsNullOrEmpty(_bearerTokenData.LoginResponseModel.RefreshToken))
                throw new InvalidOperationException($"{nameof(IBearerTokenDataProvider.LoginResponseModel.RefreshToken)} is null or empty, can't refresh token.");

            using (var client = new HttpClient())
            {
                BuildDefaultHeaders(client);

                using (var content = new FormUrlEncodedContent(BuildRefreshTokenPostParameters()))
                {
                    var result = await client.PostAsync(BuildTokenURL(), content);

                    _logger.LogInformation($"{_classRefreshTokenMessageDiagnose} {_methodRefreshTokenMessageDiagnose}, {(result != null ? $"Token refresh result code: {result.StatusCode}" : "Token refresh is null")}");

                    result.EnsureSuccessStatusCode();

                    var loginResponseModel = await JsonSerializer.DeserializeAsync<LoginResponseModel>(await result.Content.ReadAsStreamAsync());

                    _logger.LogDebug($"{_classRefreshTokenMessageDiagnose} {_methodRefreshTokenMessageDiagnose}, login response model deserialized/Token refresh");

                    loginResponseModel.IssuedDateTime = Convert.ToDateTime(loginResponseModel.Issued);
                    loginResponseModel.RefreshExpiresDateTime = Convert.ToDateTime(loginResponseModel.RefreshExpires);
                    loginResponseModel.ExpiresDateTime = Convert.ToDateTime(loginResponseModel.RefreshExpires);

                    LogLoginRespondeModel(loginResponseModel);

                    _bearerTokenData.SetLoginResponseModel(loginResponseModel);
                }
            }
        }

        private void BuildDefaultHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
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
                        _logger.LogDebug($"{_classRefreshTokenMessageDiagnose} {_methodBuildDefaultHeadersMessageDiagnose}, default header assigned key: {defaultRequestHeader.Key}, value: {value}");
                    }
                }
            }
        }

        private Dictionary<string, string> BuildRefreshTokenPostParameters()
        {
            var result = new Dictionary<string, string>
            {
                { _GRANT_TYPE_POST_KEY, _GRANT_TYPE_REFRESH_TOKEN_POST_VALUE },
                { _GRANT_TYPE_REFRESH_TOKEN_POST_VALUE, _bearerTokenData.LoginResponseModel.RefreshToken }
            };

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                foreach (var item in result)
                {
                    _logger.LogDebug($"{_classRefreshTokenMessageDiagnose} {_methodRefreshTokenPostParametersMessageDiagnose}, parameters post key: {item.Key}, value: {item.Value}");
                }
            }

            return result;
        }

        private string BuildTokenURL()
        {
            var tokenURL = $"{_loginRepositorySettings.BaseUrl}/token";

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"{_classRefreshTokenMessageDiagnose} {_methodBuildTokenURLMessageDiagnose}, tokenURL: {tokenURL}");
            }

            return tokenURL;
        }

        private void LogLoginRespondeModel(LoginResponseModel loginResponseModel)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.AccesToken)}: {loginResponseModel.AccesToken}");
                _logger.LogDebug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.Expires)}: {loginResponseModel.Expires}");
                _logger.LogDebug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.ExpiresDateTime)}: {loginResponseModel.ExpiresDateTime}");
                _logger.LogDebug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.ExpiresIn)}: {loginResponseModel.ExpiresIn}");
                _logger.LogDebug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.Issued)}: {loginResponseModel.Issued}");
                _logger.LogDebug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.IssuedDateTime)}: {loginResponseModel.IssuedDateTime}");
                _logger.LogDebug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.RefreshExpires)}: {loginResponseModel.RefreshExpires}");
                _logger.LogDebug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.RefreshExpiresDateTime)}: {loginResponseModel.RefreshExpiresDateTime}");
                _logger.LogDebug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.RefreshToken)}: {loginResponseModel.RefreshToken}");
                _logger.LogDebug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.TokenType)}: {loginResponseModel.TokenType}");
            }
        }
    }
}

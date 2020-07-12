using IOLApiClient.Auth.Repository.Abstractions.Configuration;
using IOLApiClient.Auth.Repository.Abstractions.Interfaces;
using IOLApiClient.Auth.Repository.Abstractions.Models;
using IOLApiClient.DataStorage.Abstractions;
using Serilog;
using Serilog.Events;
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


        const string _GRANT_TYPE_REFRESH_TOKEN_POST_VALUE = "refresh_token";
        const string _GRANT_TYPE_POST_KEY = "grant_type";

        private readonly ILoginRepositorySettings _loginRepositorySettings;
        private readonly IBearerTokenDataProvider _bearerTokenData;
        private readonly ILogger _logger;

        public RefreshTokenRepository(
            ILoginRepositorySettings loginRepositorySettings,
            IBearerTokenDataProvider bearerTokenData,
            ILogger logger
            )
        {
            _loginRepositorySettings = loginRepositorySettings;
            _bearerTokenData = bearerTokenData;
            _logger = logger;
        }

        public async Task RefreshToken()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET-Core-3.1-client");
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

                var parameters = new Dictionary<string, string>
                {
                    { _GRANT_TYPE_POST_KEY, _GRANT_TYPE_REFRESH_TOKEN_POST_VALUE },
                    { _GRANT_TYPE_REFRESH_TOKEN_POST_VALUE, _bearerTokenData.LoginResponseModel.RefreshToken }
                };

                using (var content = new FormUrlEncodedContent(parameters))
                {
                    var result = await client.PostAsync(BuildTokenURL(), content);

                    _logger.Information($"{_classRefreshTokenMessageDiagnose} {_methodRefreshTokenMessageDiagnose}, {(result != null ? $"Token refresh result code: {result.StatusCode}" : "Token refresh is null")}");

                    result.EnsureSuccessStatusCode();

                    var loginResponseModel = await JsonSerializer.DeserializeAsync<LoginResponseModel>(await result.Content.ReadAsStreamAsync());

                    _logger.Debug($"{_classRefreshTokenMessageDiagnose} {_methodRefreshTokenMessageDiagnose}, login response model deserialized/Token refresh");

                    loginResponseModel.IssuedDateTime = Convert.ToDateTime(loginResponseModel.Issued);
                    loginResponseModel.RefreshExpiresDateTime = Convert.ToDateTime(loginResponseModel.RefreshExpires);
                    loginResponseModel.ExpiresDateTime = Convert.ToDateTime(loginResponseModel.RefreshExpires);

                    LogLoginRespondeModel(loginResponseModel);

                    _bearerTokenData.SetLoginResponseModel(loginResponseModel);
                }
            }
        }

        private string BuildTokenURL()
        {
            var tokenURL = $"{_loginRepositorySettings.BaseUrl}/token";

            if (_logger.IsEnabled(LogEventLevel.Debug))
            {
                _logger.Debug($"{_classRefreshTokenMessageDiagnose} {_methodBuildTokenURLMessageDiagnose}, tokenURL: {tokenURL}");
            }

            return tokenURL;
        }

        private void LogLoginRespondeModel(LoginResponseModel loginResponseModel)
        {
            if (_logger.IsEnabled(LogEventLevel.Debug))
            {
                _logger.Debug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.AccesToken)}: {loginResponseModel.AccesToken}");
                _logger.Debug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.Expires)}: {loginResponseModel.Expires}");
                _logger.Debug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.ExpiresDateTime)}: {loginResponseModel.ExpiresDateTime}");
                _logger.Debug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.ExpiresIn)}: {loginResponseModel.ExpiresIn}");
                _logger.Debug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.Issued)}: {loginResponseModel.Issued}");
                _logger.Debug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.IssuedDateTime)}: {loginResponseModel.IssuedDateTime}");
                _logger.Debug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.RefreshExpires)}: {loginResponseModel.RefreshExpires}");
                _logger.Debug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.RefreshExpiresDateTime)}: {loginResponseModel.RefreshExpiresDateTime}");
                _logger.Debug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.RefreshToken)}: {loginResponseModel.RefreshToken}");
                _logger.Debug($"{_classRefreshTokenMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.TokenType)}: {loginResponseModel.TokenType}");
            }
        }
    }
}

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
    public class LoginRepository : ILoginRepository
    {
        static string _classLogMessageDiagnose = $"Class: {nameof(LoginRepository)}";
        static string _methodLoginMessageDiagnose = $"Method: {nameof(Login)}";
        static string _methodBuildDefaultHeadersMessageDiagnose = $"Method: {nameof(BuildDefaultHeaders)}";
        static string _methodBuildLoginPostParametersMessageDiagnose = $"Method: {nameof(BuildLoginPostParameters)}";
        static string _methodBuildLoginContentParametersMessageDiagnose = $"Method: {nameof(BuildLoginContent)}";
        static string _methodBuildTokenURLMessageDiagnose = $"Method: {nameof(BuildTokenURL)}";
        static string _methodLogLoginRespondeModelMessageDiagnose = $"Method: {nameof(LogLoginRespondeModel)}";

        const string _USER_NAME_POST_KEY = "username";
        const string _PASSWORD_POST_KEY = "password";

        const string _GRANT_TYPE_POST_KEY = "grant_type";
        const string _GRANT_TYPE_LOGIN_POST_VALUE = "password";

        private readonly ILoginRepositorySettings _loginRepositorySettings;
        private readonly IBearerTokenDataProvider _bearerTokenData;
        private readonly ILogger _logger;

        public LoginRepository(
            ILoginRepositorySettings loginRepositorySettings,
            IBearerTokenDataProvider bearerTokenData,
            ILogger logger
            )
        {
            _loginRepositorySettings = loginRepositorySettings;
            _bearerTokenData = bearerTokenData;
            _logger = logger;
        }

        public async Task Login()
        {
            _logger.Information($"{_classLogMessageDiagnose} {_methodLoginMessageDiagnose}, Initializing login...");

            using (var client = new HttpClient())
            {
                BuildDefaultHeaders(client);
                var parameters = BuildLoginPostParameters();

                using (var content = new FormUrlEncodedContent(parameters))
                {
                    BuildLoginContent(content);

                    var result = await client.PostAsync(BuildTokenURL(), content);

                    _logger.Information($"{_classLogMessageDiagnose} {_methodLoginMessageDiagnose}, {(result != null ? $"Login result code: {result.StatusCode}" : "Login result is null")}");

                    result.EnsureSuccessStatusCode();

                    var loginResponseModel = await JsonSerializer.DeserializeAsync<LoginResponseModel>(await result.Content.ReadAsStreamAsync());

                    _logger.Debug($"{_classLogMessageDiagnose} {_methodLoginMessageDiagnose}, login responde model deserialized");

                    loginResponseModel.IssuedDateTime = Convert.ToDateTime(loginResponseModel.Issued);
                    loginResponseModel.RefreshExpiresDateTime = Convert.ToDateTime(loginResponseModel.RefreshExpires);
                    loginResponseModel.ExpiresDateTime = Convert.ToDateTime(loginResponseModel.RefreshExpires);

                    LogLoginRespondeModel(loginResponseModel);

                    _bearerTokenData.SetLoginResponseModel(loginResponseModel);
                }
            }
        }

        private void LogLoginRespondeModel(LoginResponseModel loginResponseModel)
        {
            if (_logger.IsEnabled(LogEventLevel.Debug))
            {
                _logger.Debug($"{_classLogMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.AccesToken)}: {loginResponseModel.AccesToken}");
                _logger.Debug($"{_classLogMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.Expires)}: {loginResponseModel.Expires}");
                _logger.Debug($"{_classLogMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.ExpiresDateTime)}: {loginResponseModel.ExpiresDateTime}");
                _logger.Debug($"{_classLogMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.ExpiresIn)}: {loginResponseModel.ExpiresIn}");
                _logger.Debug($"{_classLogMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.Issued)}: {loginResponseModel.Issued}");
                _logger.Debug($"{_classLogMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.IssuedDateTime)}: {loginResponseModel.IssuedDateTime}");
                _logger.Debug($"{_classLogMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.RefreshExpires)}: {loginResponseModel.RefreshExpires}");
                _logger.Debug($"{_classLogMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.RefreshExpiresDateTime)}: {loginResponseModel.RefreshExpiresDateTime}");
                _logger.Debug($"{_classLogMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.RefreshToken)}: {loginResponseModel.RefreshToken}");
                _logger.Debug($"{_classLogMessageDiagnose} {_methodLogLoginRespondeModelMessageDiagnose}, parameter {nameof(LoginResponseModel.TokenType)}: {loginResponseModel.TokenType}");
            }
        }

        private string BuildTokenURL()
        {
            var tokenURL = $"{_loginRepositorySettings.BaseUrl}/token";

            if (_logger.IsEnabled(LogEventLevel.Debug))
            {
                _logger.Debug($"{_classLogMessageDiagnose} {_methodBuildTokenURLMessageDiagnose}, tokenURL: {tokenURL}");
            }

            return tokenURL;
        }

        private void BuildLoginContent(FormUrlEncodedContent content)
        {
            content.Headers.Clear();
            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            if (_logger.IsEnabled(LogEventLevel.Debug))
            {
                foreach (var header in content.Headers)
                {
                    IEnumerable<string> values = header.Value;

                    foreach (var value in values)
                    {
                        _logger.Debug($"{_classLogMessageDiagnose} {_methodBuildDefaultHeadersMessageDiagnose}, content header assigned key: {header.Key}, value: {value}");
                    }
                }
            }
        }

        private Dictionary<string, string> BuildLoginPostParameters()
        {
            var result = new Dictionary<string, string>
            {
                { _USER_NAME_POST_KEY, _loginRepositorySettings.UserNameIOLClient },
                { _PASSWORD_POST_KEY, _loginRepositorySettings.PasswordIOLClient },
                { _GRANT_TYPE_POST_KEY, _GRANT_TYPE_LOGIN_POST_VALUE }
            };

            if (_logger.IsEnabled(LogEventLevel.Debug))
            {
                foreach (var item in result)
                {
                    _logger.Debug($"{_classLogMessageDiagnose} {_methodBuildLoginPostParametersMessageDiagnose}, parameters post key: {item.Key}, value: {item.Value}");
                }
            }

            return result;
        }

        private void BuildDefaultHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
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
                        _logger.Debug($"{_classLogMessageDiagnose} {_methodBuildDefaultHeadersMessageDiagnose}, default header assigned key: {defaultRequestHeader.Key}, value: {value}");
                    }
                }
            }
        }
    }
}
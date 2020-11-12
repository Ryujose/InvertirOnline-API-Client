using IOLApiClient.Auth.Repository.Abstractions.Models;
using IOLApiClient.DataStorage.Abstractions;
using Microsoft.Extensions.Logging;

namespace IOLApiClient.DataStorage
{
    public class BearerTokenData : IBearerTokenDataProvider
    {
        static string _classLogMessageDiagnose = $"Class: {nameof(BearerTokenData)}";
        static string _propertyLoginMessageDiagnose = $"Property: {nameof(LoginResponseModel)}";


        private readonly ILogger<BearerTokenData> _logger;
        public BearerTokenData(ILogger<BearerTokenData> logger)
        {
            _logger = logger;
        }

        private LoginResponseModel _loginResponseModel;

        public LoginResponseModel LoginResponseModel
        {
            get
            {
                LogLoginRespondeModel(_loginResponseModel, "GETTING VALUE");

                return _loginResponseModel;
            }
            set
            {
                _loginResponseModel = value;

                LogLoginRespondeModel(_loginResponseModel, "VALUE SET");
            }
        }

        private void LogLoginRespondeModel(LoginResponseModel loginResponseModel, string getSetNotification)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"{_classLogMessageDiagnose} {_propertyLoginMessageDiagnose}, property notification: {getSetNotification}");

                if (loginResponseModel == null)
                {
                    _logger.LogDebug($"{_classLogMessageDiagnose} {_propertyLoginMessageDiagnose}, loginRespondeModel: is null");

                    return;
                }

                _logger.LogDebug($"{_classLogMessageDiagnose} {_propertyLoginMessageDiagnose}, parameter {nameof(LoginResponseModel.AccesToken)}: {loginResponseModel.AccesToken}");
                _logger.LogDebug($"{_classLogMessageDiagnose} {_propertyLoginMessageDiagnose}, parameter {nameof(LoginResponseModel.Expires)}: {loginResponseModel.Expires}");
                _logger.LogDebug($"{_classLogMessageDiagnose} {_propertyLoginMessageDiagnose}, parameter {nameof(LoginResponseModel.ExpiresDateTime)}: {loginResponseModel.ExpiresDateTime}");
                _logger.LogDebug($"{_classLogMessageDiagnose} {_propertyLoginMessageDiagnose}, parameter {nameof(LoginResponseModel.ExpiresIn)}: {loginResponseModel.ExpiresIn}");
                _logger.LogDebug($"{_classLogMessageDiagnose} {_propertyLoginMessageDiagnose}, parameter {nameof(LoginResponseModel.Issued)}: {loginResponseModel.Issued}");
                _logger.LogDebug($"{_classLogMessageDiagnose} {_propertyLoginMessageDiagnose}, parameter {nameof(LoginResponseModel.IssuedDateTime)}: {loginResponseModel.IssuedDateTime}");
                _logger.LogDebug($"{_classLogMessageDiagnose} {_propertyLoginMessageDiagnose}, parameter {nameof(LoginResponseModel.RefreshExpires)}: {loginResponseModel.RefreshExpires}");
                _logger.LogDebug($"{_classLogMessageDiagnose} {_propertyLoginMessageDiagnose}, parameter {nameof(LoginResponseModel.RefreshExpiresDateTime)}: {loginResponseModel.RefreshExpiresDateTime}");
                _logger.LogDebug($"{_classLogMessageDiagnose} {_propertyLoginMessageDiagnose}, parameter {nameof(LoginResponseModel.RefreshToken)}: {loginResponseModel.RefreshToken}");
                _logger.LogDebug($"{_classLogMessageDiagnose} {_propertyLoginMessageDiagnose}, parameter {nameof(LoginResponseModel.TokenType)}: {loginResponseModel.TokenType}");
            }
        }

        public void SetLoginResponseModel(LoginResponseModel loginResponseModel)
        {
            LoginResponseModel = loginResponseModel;
        }
    }
}

using IOLApiClient.Auth.Repository.Abstractions.Configuration;
using IOLApiClient.Example.Presentation.Abstractions;
using Microsoft.Extensions.Configuration;

namespace IOLApiClient.Example.Presentation
{
    public class AppSettingsConfiguration : IAppSettingsConfiguration, ILoginRepositorySettings
    {
        public string EnvironmentName { get; set; }

        public string UserNameIOLClient { get; set; }
        public string PasswordIOLClient { get; set; }

        public string BaseUrl { get; set; }

        private readonly IConfigurationBuilder _configurationBuilder;

        public AppSettingsConfiguration(IConfigurationBuilder ConfigurationBuilder)
        {
            _configurationBuilder = ConfigurationBuilder;

            SetParameters();
        }

        private void SetParameters()
        {
            EnvironmentName = _configurationBuilder.AddEnvironmentVariables().Build()["RUNTIME_ENVIRONMENT"];

            var configuration = _configurationBuilder
                .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"AppSettings.{EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            UserNameIOLClient = configuration["appsettings:UserNameIOLClient"];
            PasswordIOLClient = configuration["appsettings:PasswordIOLClient"];
            BaseUrl = configuration["appsettings:BaseUrl"];
        }
    }
}

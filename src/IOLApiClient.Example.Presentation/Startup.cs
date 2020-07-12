using IOLApiClient.Auth.Repository.Abstractions.Configuration;
using IOLApiClient.Auth.Repository.Abstractions.Interfaces;
using IOLApiClient.Auth.Repository.Repositories;
using IOLApiClient.DataStorage;
using IOLApiClient.DataStorage.Abstractions;
using IOLApiClient.Example.Presentation.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System;

namespace IOLApiClient.Example.Presentation
{
    public static class Startup
    {
        public static IServiceProvider ServiceProvider;

        public static void RegisterDependencies()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddSingleton<ILoginRepository, LoginRepository>()
                .AddSingleton<IRefreshTokenRepository, RefreshTokenRepository>()
                .AddSingleton<IBearerTokenDataProvider, BearerTokenData>()
                .AddTransient<IConfigurationBuilder, ConfigurationBuilder>()
                .AddSingleton<ILogger>(new LoggerConfiguration()
                    .WriteTo.Console(
                        restrictedToMinimumLevel: LogEventLevel.Debug)
                    .CreateLogger());

            serviceCollection.AddSingleton<AppSettingsConfiguration>();
            serviceCollection.AddSingleton<IAppSettingsConfiguration>(a => a.GetService<AppSettingsConfiguration>());
            serviceCollection.AddSingleton<ILoginRepositorySettings>(a => a.GetService<AppSettingsConfiguration>());

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}

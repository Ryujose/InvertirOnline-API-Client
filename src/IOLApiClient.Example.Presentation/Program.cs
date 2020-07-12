using IOLApiClient.Auth.Repository.Abstractions.Interfaces;
using IOLApiClient.Example.Presentation.Abstractions.Enums;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Threading.Tasks;

namespace IOLApiClient.Example.Presentation
{
    class Program
    {
        static void Main(string[] args)
        {
            Startup.RegisterDependencies();

            var loginRepository = Startup.ServiceProvider.GetService<ILoginRepository>();
            var refreshTokenRepository = Startup.ServiceProvider.GetService<IRefreshTokenRepository>();
            var logger = Startup.ServiceProvider.GetService<ILogger>();

            string textInput;

            Console.WriteLine($"Welcome to IOLApiClient example app. {Environment.NewLine}Follow the examples and WARNING!!!! be carefully if you're using your production account on active trading hours!{Environment.NewLine}");
            Console.WriteLine($"Follow the next instructions to exit the app or do your respective testing...{Environment.NewLine}");

            Console.WriteLine($"Write :q to exit this loop and terminate this test app.{Environment.NewLine}");

            do
            {
                foreach (var value in Enum.GetValues(typeof(IOLApiClientExampleTests)))
                {
                    Console.WriteLine($"Write number: {(int)value}, to initialize the test: {((IOLApiClientExampleTests)value)}");
                }

                textInput = Console.ReadLine();

                if (textInput == ":q")
                    break;

                var testNumber = (IOLApiClientExampleTests)Convert.ToInt32(textInput);

                switch (testNumber)
                {
                    case IOLApiClientExampleTests.Login:
                        Task.Run(async () =>
                        {
                            try
                            {
                                await loginRepository.Login();
                            }
                            catch (Exception ex)
                            {
                                logger.Information(ex, "An error ocurred on API login");
                            }
                        }).Wait();
                        break;
                    case IOLApiClientExampleTests.LoginAndRefreshToken:
                        Task.Run(() =>
                        {
                            try
                            {
                                loginRepository.Login().Wait();
                                refreshTokenRepository.RefreshToken().Wait();
                            }
                            catch (Exception ex)
                            {
                                logger.Information(ex, "An error ocurred on API login");
                            }
                        }).Wait();
                        break;
                    default:
                        logger.Information($"Invalid test parameter {testNumber}");
                        break;
                } 
            } while (textInput != ":q");

            Console.WriteLine($"Exiting app, press any key to continue...");

            Console.ReadLine();
        }

    }
}

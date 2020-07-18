using IOLApiClient.Auth.Repository.Abstractions.Interfaces;
using IOLApiClient.Communication.Abstractions.Constants;
using IOLApiClient.Example.Presentation.Abstractions.Enums;
using IOLApiClient.MyAccount.Operations.Repository.Abstractions.Interfaces.V2;
using IOLApiClient.Operative.Abstractions.Models;
using IOLApiClient.Operative.Buy.Repository.Abstractions.Interfaces.V2;
using IOLApiClient.Operative.Sell.Repository.Abstractions.Interfaces.V2;
using IOLApiClient.Operative.Subscription.Repository.Abstractions.Interfaces.V2;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Linq;
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
            var buyRepository = Startup.ServiceProvider.GetService<IBuyRepository>();
            var sellRepository = Startup.ServiceProvider.GetService<ISellRepository>();
            var operationsRepository = Startup.ServiceProvider.GetService<IOperationsRepository>();
            var subscriptionRepository = Startup.ServiceProvider.GetService<ISubscriptionRepository>();
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
                                logger.Information(ex, "An error ocurred");
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
                                logger.Information(ex, "An error ocurred");
                            }
                        }).Wait();
                        break;
                    case IOLApiClientExampleTests.LoginAndBuy:
                        Task.Run(async () =>
                        {
                            try
                            {
                                loginRepository.Login().Wait();
                                var result = await buyRepository.Buy(new OperativeModel
                                {
                                    Market = Markets.BCBA,
                                    Price = 100,
                                    Quantity = 1,
                                    Symbol = "GGAL",
                                    Term = Terms.FOURTY_EIGHT_HOURS,
                                    ValidityDate = DateTime.Now.AddDays(1)
                                });

                                var message = result?.Messages == null ? null : result.Messages.Select(message => $"Title: {message.Title}, Description: {message.Description}");

                                logger.Information($"{result.IsOk}, {(message == null ? "No message" : string.Join(", ", message))}");
                            }
                            catch (Exception ex)
                            {
                                logger.Information(ex, "An error ocurred");
                            }
                        }).Wait();
                        break;
                    case IOLApiClientExampleTests.LoginAndSell:
                        Task.Run(async () =>
                        {
                            try
                            {
                                loginRepository.Login().Wait();
                                var result = await sellRepository.Sell(new OperativeModel
                                {
                                    Market = Markets.BCBA,
                                    Price = 3,
                                    Quantity = 1,
                                    Symbol = "COME",
                                    Term = Terms.FOURTY_EIGHT_HOURS,
                                    ValidityDate = DateTime.Now.AddDays(1)
                                });

                                var message = result?.Messages == null ? null : result.Messages.Select(message => $"Title: {message.Title}, Description: {message.Description}");

                                logger.Information($"{result.IsOk}, {(message == null ? "No message" : string.Join(", ", message))}");
                            }
                            catch (Exception ex)
                            {
                                logger.Information(ex, "An error ocurred");
                            }
                        }).Wait();
                        break;
                    case IOLApiClientExampleTests.LoginAndBuyAndCancelTransaction:
                        Task.Run(async () =>
                        {
                            try
                            {
                                loginRepository.Login().Wait();
                                var result = await buyRepository.Buy(new OperativeModel
                                {
                                    Market = Markets.BCBA,
                                    Price = 30,
                                    Quantity = 1,
                                    Symbol = "ALUA",
                                    Term = Terms.FOURTY_EIGHT_HOURS,
                                    ValidityDate = DateTime.Now.AddDays(1)
                                });

                                var message = result?.Messages == null ? null : result.Messages.Select(message => $"Title: {message.Title}, Description: {message.Description}");

                                logger.Information($"{result.IsOk}, {(message == null ? "No message" : string.Join(", ", message))}");

                                result = await operationsRepository.DeleteTransaction(Convert.ToInt32(result.Messages.Select(a => a.Description).Single()));

                                message = result?.Messages == null ? null : result.Messages.Select(message => $"Title: {message.Title}, Description: {message.Description}");

                                logger.Information($"{result.IsOk}, {(message == null ? "No message" : string.Join(", ", message))}");
                            }
                            catch (Exception ex)
                            {
                                logger.Information(ex, "An error ocurred");
                            }
                        }).Wait();
                        break;
                    case IOLApiClientExampleTests.LoginAndSubscription:
                        Task.Run(async () =>
                        {
                            try
                            {
                                loginRepository.Login().Wait();
                                var result = await subscriptionRepository.SubscribeFCI(new OperativeFCIModel
                                {
                                    Amount = "100",
                                    OnlyValidate = "true",
                                    Symbol = "CRTAFAA"
                                });

                                var message = result?.Messages == null ? null : result.Messages.Select(message => $"Title: {message.Title}, Description: {message.Description}");

                                logger.Information($"{result.IsOk}, {(message == null ? "No message" : string.Join(", ", message))}");
                            }
                            catch (Exception ex)
                            {
                                logger.Information(ex, "An error ocurred");
                            }
                        }).Wait();
                        break;
                    case IOLApiClientExampleTests.LoginAndBuyAndGetOperation:
                        Task.Run(async () =>
                        {
                            try
                            {
                                loginRepository.Login().Wait();

                                Console.WriteLine("Insert transaction id:");
                                int transactionID = Convert.ToInt32(Console.ReadLine());

                                var transactionDataModel = await operationsRepository.GetTransaction(Convert.ToInt32(transactionID));
                            }
                            catch (Exception ex)
                            {
                                logger.Information(ex, "An error ocurred");
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

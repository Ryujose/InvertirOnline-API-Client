using IOLApiClient.Auth.Repository.Abstractions.Interfaces;
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
            var logger = Startup.ServiceProvider.GetService<ILogger>();
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

            Console.ReadLine();
        }

    }
}

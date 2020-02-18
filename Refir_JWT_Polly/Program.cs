using Microsoft.Extensions.Configuration;
using Refit_JWT_Polly.Clients;
using Refit_JWT_Polly.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Refit_JWT_Polly
{
    class Program
    {
        private static async Task Interrupt()
        {
            await Console.Out.WriteLineAsync("Press any key to continue ...");
            Console.ReadKey();
        }

        static async Task Main()
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile($"appsettings.json");
            var config = builder.Build();

            var apiProductsClient = new ProductClientApi(config);
            await apiProductsClient.Authenticate();
            if (apiProductsClient.IsAuthenticatedUsingToken)
            { 
                await apiProductsClient.IncludeProduct(
                new Product()
                {
                    BarCode = "00005",
                    Name = "Test Product 05",
                    Price = 5.05
                });
            await Interrupt();

                await apiProductsClient.IncludeProduct(
                    new Product()
                    {
                        BarCode = "00006",
                        Name = "Test product 06",
                        Price = 6.78
                    });
                await Interrupt();
            }
            await Console.Out.WriteLineAsync("\nFinished!");
        }
    }
}

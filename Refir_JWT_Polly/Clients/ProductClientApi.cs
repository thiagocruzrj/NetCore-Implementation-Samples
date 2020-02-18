using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Retry;
using Refit_JWT_Polly.Extensions;
using Refit_JWT_Polly.Interfaces;
using Refit_JWT_Polly.Models;
using Refit;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;

namespace Refit_JWT_Polly.Clients
{
    public class ProductClientApi
    {
        private ILoginApi _loginApi;
        private IProductsApi _productsApi;
        private IConfiguration _configuration;
        private Token _token;
        private AsyncRetryPolicy _jwtPolicy;

        public bool IsAuthenticatedUsingToken { get => _token?.Authenticated ?? false ;}

        public ProductClientApi(IConfiguration configuration)
        {
            _configuration = configuration;
            string urlBase = _configuration.GetSection("APIProducts_Access:UrlBase").Value;

            _loginApi = RestService.For<ILoginApi>(urlBase);
            _productsApi = RestService.For<IProductsApi>(urlBase);
            _jwtPolicy = CreateAccessTokenPolicy();
        }

        public Task Authenticate()
        {
            try
            {
                _token = _loginApi.PostCredentials(
                    new User()
                    {
                        UserId = _configuration.GetSection("ApiProducts_Access:UserId").Value,
                        Password = _configuration.GetSection("ApiProducts_Access:Password").Value
                    }).Result;
                return Console.Out.WriteLineAsync(JsonSerializer.Serialize(_token));
            }
            catch
            {
                _token = null;
                return Console.Out.WriteLineAsync("Fail to authenticate ...");
            }
        }

        public Task IncludeProduct(Product product)
        {
            var inclusion = _jwtPolicy.ExecuteWithTokenAsync<ApiProductsResult>(
                _token, async (Context) =>
                {
                    var result = await _productsApi.IncludeProduct(
                        $"Bearer {Context["AccessToken"]}", product);

                    return result;
                });
            return Console.Out.WriteLineAsync(JsonSerializer.Serialize(inclusion.Result));
        }

        public Task ListProducts()
        {
            var consult = _jwtPolicy.ExecuteWithTokenAsync<List<Product>>(
                _token, async (context) =>
                {
                    var result = await _productsApi.ListProducts(
                        $"Bearer {context["AccessToken"]}");
                    return result;
                });

            return Console.Out.WriteLineAsync("Products registered: " + JsonSerializer.Serialize(consult.Result));
        }

        private AsyncRetryPolicy CreateAccessTokenPolicy()
        {
            return Policy
                .HandleInner<ApiException>(
                ex => ex.StatusCode == HttpStatusCode.Unauthorized)
                .RetryAsync(1, async (ex, retryCount, context) =>
                {
                    var previoslyColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Green;
                    await Console.Out.WriteAsync("Execution of RetryPolicy ...");
                    Console.ForegroundColor = previoslyColor;

                    await Authenticate();
                    if (!(_token?.Authenticated ?? false))
                        throw new InvalidOperationException("Invalid token!");

                    context["AccessToken"] = _token.AccessToken;
                });
        }
    }
}

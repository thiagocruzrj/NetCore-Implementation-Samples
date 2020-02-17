using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Retry;
using Refir_JWT_Polly.Extensions;
using Refir_JWT_Polly.Interfaces;
using Refir_JWT_Polly.Models;
using Refit;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Refir_JWT_Polly.Clients
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

        private AsyncRetryPolicy CreateAccessTokenPolicy()
        {
            throw new NotImplementedException();
        }
    }
}

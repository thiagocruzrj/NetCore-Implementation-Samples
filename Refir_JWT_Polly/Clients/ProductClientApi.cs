using Microsoft.Extensions.Configuration;
using Polly.Retry;
using Refir_JWT_Polly.Interfaces;
using Refir_JWT_Polly.Models;
using Refit;
using System;

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

        private AsyncRetryPolicy CreateAccessTokenPolicy()
        {
            throw new NotImplementedException();
        }
    }
}

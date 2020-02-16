using Refir_JWT_Polly.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Refir_JWT_Polly.Interfaces
{
    public interface IProductsApi
    {
        [Get("/Products")]
        Task<List<Product>> ListProducts(
            [Header("Authorization")] string token);

        [Post("/Product")]
        Task<ApiProductsResult> IncludeProduct(
            [Header("Authorization")] string token,
            [Body]Product product);
    }
}

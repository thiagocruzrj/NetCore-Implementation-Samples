using Refit_JWT_Polly.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Refit_JWT_Polly.Interfaces
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

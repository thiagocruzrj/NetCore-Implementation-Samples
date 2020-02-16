using Refir_JWT_Polly.Models;
using Refit;
using System.Threading.Tasks;

namespace Refir_JWT_Polly.Interfaces
{
    public interface ILoginApi
    {
        [Post("/login")]
        Task<Token> PostCredentials(User user);
    }
}

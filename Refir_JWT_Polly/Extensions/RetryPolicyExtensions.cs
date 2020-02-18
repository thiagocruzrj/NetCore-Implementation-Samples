using Polly;
using Polly.Retry;
using Refit_JWT_Polly.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Refit_JWT_Polly.Extensions
{
    public static class RetryPolicyExtensions
    {
        public static Task<T> ExecuteWithTokenAsync<T>(
            this AsyncRetryPolicy retryPolicy,
            Token token,
            Func<Context, Task<T>> action)
        {
            return retryPolicy.ExecuteAsync(action, new Dictionary<string, object>
            {
                {"AccessToken", token.AccessToken }
            });
        }
    }
}

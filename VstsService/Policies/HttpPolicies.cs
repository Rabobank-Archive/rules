using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;

namespace SecurePipelineScan.VstsService.Policies
{
    public static class HttpPolicies
    {
        private static readonly HttpStatusCode[] HttpStatusCodesWorthRetrying =
        {
            HttpStatusCode.RequestTimeout, // 408
            HttpStatusCode.InternalServerError, // 500
            HttpStatusCode.BadGateway, // 502
            HttpStatusCode.ServiceUnavailable, // 503
            HttpStatusCode.GatewayTimeout // 504
        };

        internal static AsyncRetryPolicy<HttpResponseMessage> RetryPolicy
        {
            get
            {
                return Policy
                    .HandleResult<HttpResponseMessage>(r => HttpStatusCodesWorthRetrying.Contains(r.StatusCode))
                    .Or<SocketException>(ex => ex.Message.Contains("No connection could be made because the target machine actively refused it")) // Sometimes occurs when AzDo is temporarily unreachable
                    .Or<TaskCanceledException>() // Occurs when a HTTP call times out
                    .WaitAndRetryAsync(9,
                        retryAttempt =>
                            TimeSpan.FromSeconds(Math.Pow(2,
                                retryAttempt)), // Max 9 retries, with exponential back-off. Maximum wait time is 2^9 seconds = 512 seconds = 8,5 minutes
                        (delegateResult, retryCount) =>
                        {
                            Debug.WriteLine(
                                $"[App|Policy]: Retry delegate fired, attempt {retryCount}");
                        });
            }
        }
    }
}
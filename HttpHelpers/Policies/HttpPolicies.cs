using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;

namespace HttpHelpers.Policies
{
    public static class HttpPolicies
    {
        const int RetryCount = 9;
        const int NumberToRaisePower = 2;

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
                    .Or<SocketException>(ex =>         // Sometimes occurs when AzDo is temporarily unreachable
                        ex.Message.Contains(
                            "No connection could be made because the target machine actively refused it") || // Message on Windows-based machine
                        ex.Message.Contains(
                            "An existing connection was forcibly closed by the remote host") || // Message in Azure Functions runtime
                        ex.Message.Contains(
                            "Kan geen verbinding maken omdat de doelcomputer de verbinding actief heeft geweigerd") || // Message on Windows-based machine NL
                       ex.Message.Contains("Connection refused")) // Message on MacOs-based machine
                    .Or<TaskCanceledException>() // Occurs when a HTTP call times out
                    .WaitAndRetryAsync(RetryCount,
                        retryAttempt =>
                            TimeSpan.FromSeconds(Math.Pow(NumberToRaisePower,
                                retryAttempt)));
            }
        }
    }
}
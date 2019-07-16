using System.Net.Http;
using Flurl.Http.Configuration;
using HttpHelpers.Handlers;

namespace DurableFunctionsAdministration.Client
{
    public class HttpClientFactory : DefaultHttpClientFactory
    {
        public override HttpClient CreateHttpClient(HttpMessageHandler handler)
        {
            return base.CreateHttpClient(new RetryHandler(handler));
        }
    }
}
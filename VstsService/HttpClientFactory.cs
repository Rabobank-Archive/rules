using System.Net.Http;
using Flurl.Http.Configuration;
using HttpHelpers.Handlers;
using SecurePipelineScan.VstsService.Handlers;

namespace SecurePipelineScan.VstsService
{
    public class HttpClientFactory : DefaultHttpClientFactory
    {
        public override HttpClient CreateHttpClient(HttpMessageHandler handler)
        {
            return base.CreateHttpClient(new RetryHandler(new NotFoundHandler(handler)));
        }
    }
}
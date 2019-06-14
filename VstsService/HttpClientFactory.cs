using System.Net.Http;
using Flurl.Http.Configuration;

namespace SecurePipelineScan.VstsService
{
    public class HttpClientFactory : DefaultHttpClientFactory
    {
        public override HttpClient CreateHttpClient(HttpMessageHandler handler)
        {
            return base.CreateHttpClient(new NotFoundHandler(handler));
        }
    }
}
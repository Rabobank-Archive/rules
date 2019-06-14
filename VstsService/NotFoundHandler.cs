using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SecurePipelineScan.VstsService
{
    public class NotFoundHandler : DelegatingHandler
    {
        public NotFoundHandler(HttpMessageHandler handler) : base(handler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                // Clear the contents from a 404 response because it deserializes to an empty object otherwise.
                result.Content = new StringContent(string.Empty);
            }
            
            return result;
        }
    }
}
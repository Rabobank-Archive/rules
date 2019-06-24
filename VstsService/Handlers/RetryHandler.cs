using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService.Policies;

namespace SecurePipelineScan.VstsService.Handlers
{
    public class RetryHandler : DelegatingHandler
    {
        public RetryHandler(HttpMessageHandler handler) : base(handler)
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            HttpPolicies.RetryPolicy.ExecuteAsync(
                ct => base.SendAsync(request, ct), cancellationToken);
    }
}
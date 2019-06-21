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

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var result =
                await HttpPolicies.RetryPolicy.ExecuteAsync(
                    ct => base.SendAsync(request, ct), cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
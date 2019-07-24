using System.Collections.Generic;
using System.Linq;
using Flurl.Http;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Enumerators
{
    public class MemberEntitlementsEnumerator<TResponse> : IVstsRequestEnumerator<TResponse>
    {
        private const int Top = 100;

        public IEnumerable<TResponse> Enumerate(IFlurlRequest request)
        {
            request.SetQueryParam("top",  Top);
            var skip = 0;
            
            var more = true;
            while (more)
            {
                var result = request.GetJsonAsync<Entitlements<TResponse>>().ConfigureAwait(false).GetAwaiter().GetResult();
                foreach (var item in result.Items)
                {
                    yield return item;
                }

                request.SetQueryParam("skip", skip += Top);
                more = result.Items.Any();
            }
        }
    }
}
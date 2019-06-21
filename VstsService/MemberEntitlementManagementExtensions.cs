using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService
{
    public static class MemberEntitlementManagementExtensions
    {
        public static IEnumerable<T> Get<T>(this IVstsRestClient client, IVstsRequest<Entitlements<T>> request, int take = 100)
        {
            request.QueryParams["take"] = take;
            var skip = 0;
            
            var more = true;
            while (more)
            {
                var result = client.GetAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
                foreach (var item in result.Items)
                {
                    yield return item;
                }

                request.QueryParams["skip"] = skip += take;
                more = result.Items.Any();
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService
{
    public static class MemberEntitlementManagementExtensions
    {
        private const int MaxIterationSize = 100;
        
        public static IEnumerable<T> Get<T>(this IVstsRestClient client, IVstsRequest<Entitlements<T>> request) =>
            Get(client, request, MaxIterationSize);

        internal static IEnumerable<T> Get<T>(this IVstsRestClient client, IVstsRequest<Entitlements<T>> request,
            int take)
        {
            request.QueryParams["top"] = take;
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
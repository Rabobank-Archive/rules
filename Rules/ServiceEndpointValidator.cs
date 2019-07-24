using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules
{
    public class ServiceEndpointValidator : IServiceEndpointValidator
    {
        private readonly IVstsRestClient _client;
        private readonly IMemoryCache _cache;

        public ServiceEndpointValidator(IVstsRestClient client, IMemoryCache cache)
        {
            _client = client;
            _cache = cache;
        }

        public async Task<bool> ScanForProductionEndpointsAsync(string project, Guid id)
        {
            // Cache the results per project to avoid stressing the REST API.
            return (await _cache.GetOrCreate(project, ResolveEndpoints(project)).ConfigureAwait(false))
                .ProductionEndpoints()
                .Any(e => (Guid)e["id"] == id);
        }

        private Func<ICacheEntry, Task<JToken>> ResolveEndpoints(string project)
        {
            return async entry =>
            {
                entry.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1);
                return await _client.GetAsync(VstsService.Requests.ServiceEndpoint.Endpoints(project).Request.AsJson())
                    .ConfigureAwait(false);
            };
        }
    }
}
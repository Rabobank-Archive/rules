using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;

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

        public bool IsProduction(string project, Guid id)
        {
            var endpoint = ResolveEndpoint(project, id);
            return IsEndpoint(endpoint) && NoTestUrl(endpoint);
        }

        private ServiceEndpoint ResolveEndpoint(string project, Guid id)
        {
            // Cache the results per project to avoid stressing the REST API.
            return _cache.GetOrCreate(project, ResolveEndpoints(project)).SingleOrDefault(x => x.Id == id);
        }

        private Func<ICacheEntry, IEnumerable<ServiceEndpoint>> ResolveEndpoints(string project)
        {
            return (entry) => _client.Get(VstsService.Requests.ServiceEndpoint.Endpoints(project)).Value;
        }

        private static bool NoTestUrl(ServiceEndpoint endpoint)
        {
            return !endpoint.Url.Contains("test");
        }

        private static bool IsEndpoint(ServiceEndpoint endpoint)
        {
            return endpoint != null;
        }
    }
}
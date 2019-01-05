using System;
using System.Linq;
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

        public bool IsProduction(string project, Guid id)
        {
            // Cache the results per project to avoid stressing the REST API.
            return _cache.GetOrCreate(project, ResolveEndpoints(project))
                .ProductionEndpoints()
                .Any(e => (Guid)e["id"] == id);
        }

        private Func<ICacheEntry, JToken> ResolveEndpoints(string project)
        {
            return entry =>
            {
                entry.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1);
                return _client.Get(VstsService.Requests.ServiceEndpoint.Endpoints(project).AsJson());
            };
        }

        public bool CheckReleaseEnvironment(string project, string releaseId, string environmentId)
        {
            var environment = _client.Get(VstsService.Requests.Release.Environment(project, releaseId, environmentId));
            return environment
                .DeployPhasesSnapshot
                .SelectMany(s => s.WorkflowTasks)
                .SelectMany(w => w.Inputs)
                .Select(i => i.Value)
                .Any(x => Guid.TryParse(x, out var id) && IsProduction(project, id));
        }
    }
}
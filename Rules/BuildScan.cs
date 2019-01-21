using System.Linq;
using Newtonsoft.Json.Linq;
using SecurePipelineScan.Rules.Events;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules
{
    public class BuildScan : IServiceHookScan<BuildScanReport>
    {
        private readonly IVstsRestClient _client;

        public BuildScan(IVstsRestClient client) 
        {
            _client = client;
        }
        
        public BuildScanReport Completed(JObject input)
        {
            var id = (string) input.SelectToken("resource.id");
            var build = _client.Get<VstsService.Response.Build>((string) input.SelectToken("resource.url"));
            var project = build.Project.Name;
            var artifacts = _client.Get(VstsService.Requests.Builds.Artifacts(project, id));
            
            return new BuildScanReport
            {
                Id = id, 
                Project = project,
                ArtifactsStoredSecure = artifacts.All(a => a.Resource.Type == "Container")
            };
        }
    }
}
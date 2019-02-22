using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Events
{
    public class BuildScan : IServiceHookScan<BuildScanReport>
    {
        private const string FortifyScaTaskId = "818386e5-c8a5-46c3-822d-954b3c8fb130";
        private const string SonarQubePublishTaskId = "291ed61f-1ee4-45d3-b1b0-bf822d9095ef";
        private readonly IVstsRestClient _client;

        public BuildScan(IVstsRestClient client)
        {
            _client = client;
        }

        public BuildScanReport Completed(JObject input)
        {
            var id = (string)input.SelectToken("resource.id");
            var build = _client.Get<VstsService.Response.Build>((string)input.SelectToken("resource.url"));

            var project = build.Project.Name;
            var timeline = _client.Get(VstsService.Requests.Builds.Timeline(project, id).AsJson());
            var usedTaskIds = timeline.SelectTokens("records[*].task.id").Values<string>();

            var artifacts = _client.Get(VstsService.Requests.Builds.Artifacts(project, id));

            return new BuildScanReport
            {
                Id = id,
                Pipeline = build.Definition.Name,
                Project = project,
                CreatedDate = (DateTime)input["createdDate"],
                ArtifactsStoredSecure = artifacts.All(a => a.Resource.Type == "Container"),
                UsesFortify = usedTaskIds.Contains(FortifyScaTaskId),
                UsesSonarQube = usedTaskIds.Contains(SonarQubePublishTaskId),
            };
        }
    }
}
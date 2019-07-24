using System;
using System.Linq;
using System.Threading.Tasks;
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

        public Task<BuildScanReport> GetCompletedReportAsync(JObject input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return GetCompletedReportInternalAsync(input);
        }

        private async Task<BuildScanReport> GetCompletedReportInternalAsync(JObject input)
        {
            var id = (string)input.SelectToken("resource.id");
            var build = await _client.GetAsync<VstsService.Response.Build>((Uri)input.SelectToken("resource.url"))
                .ConfigureAwait(false);

            if (!build.Result.Equals("succeeded", StringComparison.OrdinalIgnoreCase))
                return null;

            var project = build.Project.Name;
            var timeline = await _client.GetAsync(VstsService.Requests.Builds.Timeline(project, id).AsJson())
                .ConfigureAwait(false);
            var usedTaskIds = timeline?.SelectTokens("records[*].task.id").Values<string>().ToList();

            var artifacts = _client.Get(VstsService.Requests.Builds.Artifacts(project, id));

            return new BuildScanReport
            {
                Id = id,
                Pipeline = build.Definition.Name,
                Project = project,
                CreatedDate = (DateTime)input["createdDate"],
                ArtifactsStoredSecure = artifacts.All(a => a.Resource.Type == "Container"),
                UsesFortify = usedTaskIds?.Contains(FortifyScaTaskId),
                UsesSonarQube = usedTaskIds?.Contains(SonarQubePublishTaskId),
            };
        }
    }
}
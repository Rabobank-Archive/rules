using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Events
{
    public class ReleaseDeploymentScan : IServiceHookScan<ReleaseDeploymentCompletedReport>
    {
        private readonly IVstsRestClient _client;

        public ReleaseDeploymentScan(IVstsRestClient client)
        {
            _client = client;
        }

        public Task<ReleaseDeploymentCompletedReport> GetCompletedReportAsync(JObject input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return GetCompletedReportInternalAsync(input);
        }

        private async Task<ReleaseDeploymentCompletedReport> GetCompletedReportInternalAsync(JObject input)
        {
            var release = await ResolveReleaseAsync(input).ConfigureAwait(false);
            var environment = await ResolveEnvironmentAsync(input).ConfigureAwait(false);
            var project = (string)input.SelectToken("resource.project.name");
            var change = GetLatestSm9Change(release);

            return new ReleaseDeploymentCompletedReport
            {
                Project = project,
                Pipeline = (string)input.SelectToken("resource.environment.releaseDefinition.name"),
                Release = (string)input.SelectToken("resource.environment.release.name"),
                ReleaseId = (string)input.SelectToken("resource.environment.release.id"),
                Environment = (string)input.SelectToken("resource.environment.name"),
                CreatedDate = (DateTime)input["createdDate"],
                HasApprovalOptions = CheckApprovalOptions(environment),
                HasBranchFilterForAllArtifacts = CheckBranchFilters(release, environment),
                UsesManagedAgentsOnly = await CheckAgentsAsync(project, environment).ConfigureAwait(false),
                AllArtifactsAreFromBuild = CheckArtifacts(release),
                SM9ChangeId = GetSm9ChangeTag(change)
            };
        }
        private static string GetLatestSm9Change(Response.Release release) => release?.Tags.LastOrDefault(x => x.Contains("SM9ChangeId"));

        private static string GetSm9ChangeTag(string change) => change?.Split(' ').Last();

        private static bool? CheckArtifacts(Response.Release release)
        {
            if (release == null)
            {
                return null;
            }

            return release.Artifacts.Any() && release.Artifacts.All(a => a.Type == "Build");
        }

        private async Task<bool?> CheckAgentsAsync(string project, Response.Environment environment)
        {
            if (environment == null)
            {
                return null;
            }

            var phasesWithAgentBasedDeployment =
                environment.DeployPhasesSnapshot.Where(p => p.PhaseType == "agentBasedDeployment").ToList();

            if (!phasesWithAgentBasedDeployment.Any())
            {
                return null;
            }

            var queues = await Task.WhenAll(phasesWithAgentBasedDeployment.Select(async p =>
                (await _client.GetAsync(VstsService.Requests.DistributedTask.AgentQueue(project,
                    p.DeploymentInput.QueueId)).ConfigureAwait(false)))).ConfigureAwait(false);

            if (queues.Any(x => x == null))
                return false;

            int[] managedPoolIds = { 114, 115, 116, 119, 120, 122, 117, 121 };
            return !queues.Select(x => x.Pool.Id).Except(managedPoolIds).Any();
        }

        private static bool? CheckBranchFilters(Response.Release release, Response.Environment environment)
        {
            if (release == null || environment == null)
            {
                return null;
            }

            return release.Artifacts.All(a =>
                environment.Conditions.Any(c => c.ConditionType == "artifact" && c.Name == a.Alias));
        }

        private async Task<Response.Release> ResolveReleaseAsync(JToken input)
        {
            var project = (string)input.SelectToken("resource.project.name");
            var releaseId = (string)input.SelectToken("resource.environment.releaseId");

            return await _client.GetAsync(VstsService.Requests.ReleaseManagement.Release(project, releaseId))
                .ConfigureAwait(false);
        }

        private static bool? CheckApprovalOptions(Response.Environment environment)
        {
            if (environment == null)
            {
                return null;
            }

            return environment.PreApprovalsSnapshot.ApprovalOptions != null &&
                   !environment.PreApprovalsSnapshot.ApprovalOptions.ReleaseCreatorCanBeApprover &&
                   environment.PreApprovalsSnapshot.Approvals.Any(approval => !approval.IsAutomated);
        }

        private async Task<Response.Environment> ResolveEnvironmentAsync(JToken input)
        {
            var project = (string)input.SelectToken("resource.project.name");
            var releaseId = (string)input.SelectToken("resource.environment.releaseId");
            var environmentId = (string)input.SelectToken("resource.environment.id");

            return await _client.GetAsync(VstsService.Requests.ReleaseManagement.Environment(project, releaseId, environmentId))
                .ConfigureAwait(false);
        }
    }
}
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
        private static readonly Guid[] IgnoredTaskIds =
        {
            new Guid("dd84dea2-33b4-4745-a2e2-d88803403c1b"), // auto-lst
            new Guid("291ed61f-1ee4-45d3-b1b0-bf822d9095ef"), // SonarQubePublish
            new Guid("d0c045b6-d01d-4d69-882a-c21b18a35472"), // SM9 - Create
            new Guid("f0d043e2-e42f-4d11-113c-d34c99d63896"), // SM9 - Close 
        };
        private readonly IServiceEndpointValidator _endpoints;
        private readonly IVstsRestClient _client;

        public ReleaseDeploymentScan(IServiceEndpointValidator endpoints, IVstsRestClient client)
        {
            _endpoints = endpoints;
            _client = client;
        }

        public async Task<ReleaseDeploymentCompletedReport> Completed(JObject input)
        {
            var release = await ResolveRelease(input);
            var environment = await ResolveEnvironment(input);
            var project = (string)input.SelectToken("resource.project.name");
            return new ReleaseDeploymentCompletedReport
            {
                Project = project,
                Pipeline = (string)input.SelectToken("resource.environment.releaseDefinition.name"),
                Release = (string)input.SelectToken("resource.environment.release.name"),
                ReleaseId = (string)input.SelectToken("resource.environment.release.id"),
                Environment = (string)input.SelectToken("resource.environment.name"),
                CreatedDate = (DateTime)input["createdDate"],
                UsesProductionEndpoints = await UsesProductionEndpoints(project, environment),
                HasApprovalOptions = CheckApprovalOptions(environment),
                HasBranchFilterForAllArtifacts = CheckBranchFilters(release, environment),
                UsesManagedAgentsOnly = await CheckAgents(project, environment),
                AllArtifactsAreFromBuild = CheckArtifacts(release), 
                RelatedToSm9Change = IsRelatedToSm9Change(release)
            };
        }

        private static bool? IsRelatedToSm9Change(Response.Release release)
        {
            return release?.Tags.Any(x => x.Contains("SM9ChangeId"));
        }

        private static bool? CheckArtifacts(Response.Release release)
        {
            if (release == null)
            {
                return null;
            }
            
            return release.Artifacts.Any() && release.Artifacts.All(a => a.Type == "Build");
        }

        private async Task<bool?> CheckAgents(string project, Response.Environment environment)
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

            var poolIds = await Task.WhenAll(phasesWithAgentBasedDeployment.Select(async p =>
                (await _client.GetAsync(VstsService.Requests.DistributedTask.AgentQueue(project, p.DeploymentInput.QueueId))).Pool.Id));
            
            int[] managedPoolIds = { 114, 115, 116, 119, 120, 122, 117, 121 };
            return !poolIds.Except(managedPoolIds).Any();
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

        private async Task<Response.Release> ResolveRelease(JObject input)
        {
            var project = (string) input.SelectToken("resource.project.name");
            var releaseId = (string) input.SelectToken("resource.environment.releaseId");

            return await _client.GetAsync(VstsService.Requests.ReleaseManagement.Release(project, releaseId));
        }

        private async Task<bool?> UsesProductionEndpoints(string project, Response.Environment environment)
        {
            if (environment == null)
            {
                return null;
            }

            return (await Task.WhenAll(environment?.DeployPhasesSnapshot
                    .SelectMany(s => s.WorkflowTasks)
                    .Where(w => !IgnoredTaskIds.Contains(w.TaskId))
                    .SelectMany(w => w.Inputs)
                    .Select(i => i.Value)
                    .Select(async x => Guid.TryParse(x, out var id) && await _endpoints.IsProduction(project, id))))
                .Any(x => x);
        }

        private static bool? CheckApprovalOptions(Response.Environment environment)
        {
            if (environment == null)
            {
                return null;
            }
            
            return !environment.PreApprovalsSnapshot.ApprovalOptions.ReleaseCreatorCanBeApprover &&
                   environment.PreApprovalsSnapshot.Approvals.Any(approval => !approval.IsAutomated);
        }

        private async Task<Response.Environment> ResolveEnvironment(JToken input)
        {
            var project = (string) input.SelectToken("resource.project.name");
            var releaseId = (string) input.SelectToken("resource.environment.releaseId");
            var environmentId = (string) input.SelectToken("resource.environment.id");

            return await _client.GetAsync(VstsService.Requests.ReleaseManagement.Environment(project, releaseId, environmentId));
        }
    }
}
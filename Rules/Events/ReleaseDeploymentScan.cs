using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Events
{
    public class ReleaseDeploymentScan : IServiceHookScan<ReleaseDeploymentCompletedReport>
    {
        private static readonly Guid[] _ignoredTaskIds =
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

        public ReleaseDeploymentCompletedReport Completed(JObject input)
        {
            var release = ResolveRelease(input);
            var environment = ResolveEnvironment(input);
            var project = (string)input.SelectToken("resource.project.name");
            var queueIds = input.SelectTokens("resource.environment.deployPhasesSnapshot[*].deploymentInput.queueId").Values<int>();
            return new ReleaseDeploymentCompletedReport
            {
                Project = project,
                Pipeline = (string)input.SelectToken("resource.environment.releaseDefinition.name"),
                Release = (string)input.SelectToken("resource.environment.release.name"),
                ReleaseId = (string)input.SelectToken("resource.environment.release.id"),
                Environment = (string)input.SelectToken("resource.environment.name"),
                CreatedDate = (DateTime)input["createdDate"],
                UsesProductionEndpoints = UsesProductionEndpoints(project, environment),
                HasApprovalOptions = CheckApprovalOptions(environment),
                HasBranchFilterForAllArtifacts = CheckBranchFilters(release, environment),
                UsesManagedAgentsOnly = CheckAgents(project, queueIds),
                AllArtifactsAreFromBuild = CheckArtifacts(release), 
                RelatedToSm9Change = IsRelatedToSm9Change(release)
            };
        }

        private bool IsRelatedToSm9Change(Response.Release release)
        {
            return release.Tags.Any(x => x.Contains("SM9ChangeId"));
        }

        private bool CheckArtifacts(Response.Release release)
        {
            return release.Artifacts.Any() && release.Artifacts.All(a => a.Type == "Build");
        }

        private bool CheckAgents(string project, IEnumerable<int> queueIds)
        {
            int[] managedPoolIds = { 114, 115, 116, 119, 120, 122, 117, 121 };
            return queueIds.All(id => managedPoolIds.Contains(_client.Get(VstsService.Requests.DistributedTask.AgentQueue(project, id)).Pool.Id));
        }

        private bool CheckBranchFilters(Response.Release release, Response.Environment environment)
        {
            return release.Artifacts.All(a =>
                environment.Conditions.Any(c => c.ConditionType == "artifact" && c.Name == a.Alias));
        }

        private Response.Release ResolveRelease(JObject input)
        {
            var project = (string) input.SelectToken("resource.project.name");
            var releaseId = (string) input.SelectToken("resource.environment.releaseId");

            return _client.Get(VstsService.Requests.Release.Releases(project, releaseId));
        }

        private bool UsesProductionEndpoints(string project, Response.Environment environment)
        {
            return environment
                .DeployPhasesSnapshot
                .SelectMany(s => s.WorkflowTasks)
                .Where(w => !_ignoredTaskIds.Contains(w.TaskId))
                .SelectMany(w => w.Inputs)
                .Select(i => i.Value)
                .Any(x => Guid.TryParse(x, out var id) && _endpoints.IsProduction(project, id));
        }

        private static bool CheckApprovalOptions(Response.Environment environment)
        {
            return !environment.PreApprovalsSnapshot.ApprovalOptions.ReleaseCreatorCanBeApprover &&
                   environment.PreApprovalsSnapshot.Approvals.Any(approval => !approval.IsAutomated);
        }

        private Response.Environment ResolveEnvironment(JToken input)
        {
            var project = (string) input.SelectToken("resource.project.name");
            var releaseId = (string) input.SelectToken("resource.environment.releaseId");
            var environmentId = (string) input.SelectToken("resource.environment.id");

            return _client.Get(VstsService.Requests.Release.Environment(project, releaseId, environmentId));
        }
    }
}
using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Events
{
    public class ReleaseDeploymentScan : IServiceHookScan<ReleaseDeploymentCompletedReport>
    {
        private readonly IServiceEndpointValidator _endpoints;
        private readonly IVstsRestClient _client;

        public ReleaseDeploymentScan(IServiceEndpointValidator endpoints, IVstsRestClient client)
        {
            _endpoints = endpoints;
            _client = client;
        }

        public ReleaseDeploymentCompletedReport Completed(JObject input)
        {
            var environment = ResolveEnvironment(input);
            var project = (string)input.SelectToken("resource.project.name");
            return new ReleaseDeploymentCompletedReport
            {
                Project = project,
                Pipeline = (string)input.SelectToken("resource.environment.releaseDefinition.name"),
                Release = (string)input.SelectToken("resource.environment.release.name"),
                ReleaseId = (string)input.SelectToken("resource.environment.release.id"),
                Environment = (string)input.SelectToken("resource.environment.name"),
                CreatedDate = (DateTime?)input["createdDate"],
                UsesProductionEndpoints = UsesProductionEndpoints(project, environment),
                HasApprovalOptions = CheckApprovalOptions(environment)
            };
        }

        private bool UsesProductionEndpoints(string project, Response.Environment environment)
        {
            return environment
                .DeployPhasesSnapshot
                .SelectMany(s => s.WorkflowTasks)
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

            var environment = _client.Get(VstsService.Requests.Release.Environment(project, releaseId, environmentId));
            return environment;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Events
{
    public class ReleaseDeploymentScan
    {
        private readonly IServiceEndpointValidator _endpoints;

        public ReleaseDeploymentScan(IServiceEndpointValidator endpoints)
        {
            _endpoints = endpoints;
        }

        public ReleaseDeploymentCompletedReport Completed(JObject input)
        {
            return new ReleaseDeploymentCompletedReport
            {
                Project = (string)input.SelectToken("resource.project.name"),
                Release = (string)input.SelectToken("resource.environment.release.name"),
                ReleaseId = (string)input.SelectToken("resource.environment.release.id"),
                Environment = (string)input.SelectToken("resource.environment.name"),
                CreatedDate = (DateTime?)input["createdDate"],
                UsesProductionEndpoints = UsesProductionEndpoints(input),
                HasApprovalOptions = CheckApprovalOptions(input)
            };
        }

        private bool UsesProductionEndpoints(JToken input)
        {
            // Could reuse the project name as resolved earlier but I like the idea of autonomy here
            var project = (string)input.SelectToken("resource.project.name");
            var releaseId = (string)input.SelectToken("resource.environment.releaseId");
            var environmentId = (string)input.SelectToken("resource.environment.id");
            
            return _endpoints.IsProductionEnvironment(project, releaseId, environmentId);
        }

        private static bool CheckApprovalOptions(JToken input)
        {
            var options = input.SelectToken("resource.environment.preApprovalsSnapshot.approvalOptions");
            return options != null &&
                   (long)options["requiredApproverCount"] > 0 &&
                   (bool?)options["releaseCreatorCanBeApprover"] == false;
        }
    }
}
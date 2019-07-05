using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Requests = SecurePipelineScan.VstsService.Requests;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Security
{
    public class PipelineHasAtLeastOneStageWithApproval : IRule
    {
        private readonly IVstsRestClient _client;

        public PipelineHasAtLeastOneStageWithApproval(IVstsRestClient client)
        {
            _client = client;
        }

        //TODO: Once we can get information from SM9 regarding the production pipeline/stage,
        //      we only have to check the approval of this stage.
        string IRule.Description => "Release pipeline contains 4-eyes approval";

        string IRule.Why =>
            "To make sure production releases are approved by at least one other person";

        public async Task<bool> Evaluate(string project, string pipelineId)
        {
            var releasePipeline = await _client.GetAsync(Requests.ReleaseManagement.Definition(project, pipelineId));
            return HasAtLeastOneStageWithApproval(releasePipeline);
        }

        private bool HasAtLeastOneStageWithApproval(ReleaseDefinition releasePipeline)
        {
            return releasePipeline
                .Environments
                .Select(p => p.PreDeployApprovals)
                .Any(p => !p.ApprovalOptions.ReleaseCreatorCanBeApprover && p.Approvals.Any(a => a.Approver != null));
        }

    }
}
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Requests = SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.Rules.Security
{
    public class PipelineHasAtLeastOneStageWithApproval : IRule
    {
        private readonly IVstsRestClient _client;

        public PipelineHasAtLeastOneStageWithApproval(IVstsRestClient client)
        {
            _client = client;
        }

        string IRule.Description => "Release pipeline contains 4-eyes approval";

        string IRule.Why =>
            "To make sure production releases are approved by at least one other person";
        bool IRule.IsSox => true;
        public async Task<bool> EvaluateAsync(string project, string releasePipelineId) //NOSONAR
        {
            var releasePipeline = await _client.GetAsync(Requests.ReleaseManagement.Definition(project, releasePipelineId))
                .ConfigureAwait(false);
            return HasAtLeastOneStageWithApproval(releasePipeline);
        }

        private static bool HasAtLeastOneStageWithApproval(ReleaseDefinition releasePipeline)
        {
            return releasePipeline
                .Environments
                .Select(p => p.PreDeployApprovals)
                .Any(p => 
                    p.ApprovalOptions != null
                    && !p.ApprovalOptions.ReleaseCreatorCanBeApprover 
                    && p.Approvals.Any(a => a.Approver != null));
        }

    }
}
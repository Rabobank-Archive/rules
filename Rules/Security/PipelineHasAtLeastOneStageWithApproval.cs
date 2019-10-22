using System;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class PipelineHasAtLeastOneStageWithApproval : IReleasePipelineRule
    {
        string IRule.Description => "Release pipeline contains 4-eyes approval";
        string IRule.Link => "https://confluence.dev.somecompany.nl/x/DGjlCw";
        bool IRule.IsSox => true;

        public Task<bool> EvaluateAsync(string projectId, ReleaseDefinition releasePipeline)
        {
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));

            var result = releasePipeline
                .Environments
                .Select(p => p.PreDeployApprovals)
                .Any(p =>
                    p.ApprovalOptions != null
                    && !p.ApprovalOptions.ReleaseCreatorCanBeApprover
                    && p.Approvals.Any(a => a.Approver != null));

            return Task.FromResult(result);
        }
    }
}
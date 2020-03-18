using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService.Response;
using Task = System.Threading.Tasks.Task;

namespace AzureDevOps.Compliancy.Rules
{
    public class PipelineHasAtLeastOneStageWithApproval : IReleasePipelineRule
    {
        [ExcludeFromCodeCoverage] string IRule.Description => "Release pipeline contains 4-eyes approval (SOx)";
        [ExcludeFromCodeCoverage] string IRule.Link => "https://confluence.dev.somecompany.nl/x/DGjlCw";

        public Task<bool?> EvaluateAsync(string projectId,
            ReleaseDefinition releasePipeline)
        {
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));

            bool? result = releasePipeline
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
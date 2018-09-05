using System.Linq;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Release
{
    public class ApprovedByNotTheSameAsRequestedFor : IReleaseRule
    {
        public bool GetResult(VstsService.Response.Release release)
        {
            return release.Environments.All(
                e => e.PreDeployApprovals == null || e.PreDeployApprovals.All(
                        p => p.IsAutomated || p.Status=="Pending" || e.DeploySteps.All(
                            d => p.ApprovedBy?.Id != d.RequestedFor?.Id)));
        }
    }
}
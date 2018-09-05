using System.Linq;

namespace SecurePipelineScan.Rules.Release
{
    public class LastModifiedByNotTheSameAsApprovedBy : IReleaseRule
    {
        public bool GetResult(SecurePipelineScan.VstsService.Response.Release release)
        {
            return release.Environments.All(
                e => e.DeploySteps.All(
                    d => e.PreDeployApprovals.All(
                        p => p.ApprovedBy.Id != d.LastModifiedBy.Id)));
        }
    }
}
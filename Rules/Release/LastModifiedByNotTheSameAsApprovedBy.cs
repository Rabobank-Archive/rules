using System.Linq;

namespace SecurePipelineScan.Rules.Release
{
    public class LastModifiedByNotTheSameAsApprovedBy
    {
        public bool GetResult(VstsService.Response.Release release)
        {
            return release.Environments == null || release.Environments.All(
                e => e.DeploySteps.All(
                    d => e.PreDeployApprovals.All(
                        p => p.ApprovedBy?.Id != d.LastModifiedBy.Id)));
        }
    }
}
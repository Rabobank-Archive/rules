using System.Linq;

namespace Rules.Rules.Release
{
    public class LastModifiedByNotTheSameAsApprovedBy : IReleaseRule
    {
        public bool GetResult(Vsts.Response.Release release)
        {
            return release.Environments.All(
                e => e.DeploySteps.All(
                    d => e.PreDeployApprovals.All(
                        p => p.ApprovedBy.Id != d.LastModifiedBy.Id)));
        }
    }
}
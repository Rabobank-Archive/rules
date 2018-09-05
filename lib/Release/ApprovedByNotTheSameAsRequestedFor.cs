using System.Linq;
using vsts.Response;

namespace lib.Rules.Release
{
    public class ApprovedByNotTheSameAsRequestedFor : IReleaseRule
    {
        public bool GetResult(vsts.Response.Release release)
        {
            return release.Environments.All(
                e => e.PreDeployApprovals.All(
                        p => p.IsAutomated || e.DeploySteps.All(
                            d => p.ApprovedBy.Id != d.RequestedFor.Id)));
        }
    }
}
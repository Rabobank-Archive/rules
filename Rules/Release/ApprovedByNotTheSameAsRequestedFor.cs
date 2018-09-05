using System.Linq;
using Vsts.Response;

namespace Rules.Rules.Release
{
    public class ApprovedByNotTheSameAsRequestedFor : IReleaseRule
    {
        public bool GetResult(Vsts.Response.Release release)
        {
            return release.Environments.All(
                e => e.PreDeployApprovals.All(
                        p => p.IsAutomated || e.DeploySteps.All(
                            d => p.ApprovedBy.Id != d.RequestedFor.Id)));
        }
    }
}
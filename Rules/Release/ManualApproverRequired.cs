using System.Linq;

namespace Rules.Rules.Release
{
    public class ManualApproverRequired : IReleaseRule
    {
        public bool GetResult(Vsts.Response.Release release)
        {
            return release.Environments.Any(
                e => e.PreDeployApprovals.Any(
                    p => !p.IsAutomated));
        }
    }
}
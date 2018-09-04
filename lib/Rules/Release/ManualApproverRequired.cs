using System.Linq;

namespace lib.Rules.Release
{
    public class ManualApproverRequired : IReleaseRule
    {
        public bool GetResult(Response.Release release)
        {
            return release.Environments.Any(
                e => e.PreDeployApprovals.Any(
                    p => !p.IsAutomated));
        }
    }
}
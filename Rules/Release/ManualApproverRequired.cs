using System.Linq;

namespace SecurePipelineScan.Rules.Release
{
    public class ManualApproverRequired : IReleaseRule
    {
        public bool GetResult(VstsService.Response.Release release)
        {
            return release.Environments == null || release.Environments.Any(
                e => e.PreDeployApprovals.Any(
                    p => !p.IsAutomated));
        }
    }
}
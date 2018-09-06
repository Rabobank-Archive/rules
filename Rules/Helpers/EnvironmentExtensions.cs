using System.Linq;

namespace SecurePipelineScan.Rules.Helpers
{
    public static class EnvironmentExtensions
    {
        public static bool IsApprovedBySomeoneElse(this SecurePipelineScan.VstsService.Response.Environment env)
        {
            if (env.PreDeployApprovals != null)
            {
                return env.PreDeployApprovals.Any(
                    p => env.DeploySteps.Any(
                        d => d.RequestedFor.Id != p.ApprovedBy.Id)
                    );
            }

            return false;
        }
    }
}
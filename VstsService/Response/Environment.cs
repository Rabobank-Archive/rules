using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class Environment
    {
        public string Id { get; set; }
        public IEnumerable<PreDeployApproval> PreDeployApprovals { get; set; }
        public IEnumerable<DeployStep> DeploySteps { get; set; }

    }
}
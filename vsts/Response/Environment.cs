using System.Collections.Generic;

namespace Vsts.Response
{
    public class Environment
    {
        public string Id { get; set; }
        public IEnumerable<PreDeployApproval> PreDeployApprovals { get; set; }
        public IEnumerable<DeployStep> DeploySteps { get; set; }

    }
}
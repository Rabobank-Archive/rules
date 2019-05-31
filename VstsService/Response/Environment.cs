using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class Environment
    {
        public int Id { get; set; }
        public IEnumerable<PreDeployApproval> PreDeployApprovals { get; set; }
        public IEnumerable<DeployStep> DeploySteps { get; set; }
        public string Name { get; set; }
        public IEnumerable<Condition> Conditions { get; set; }
        public IEnumerable<DeployPhaseSnapshot> DeployPhasesSnapshot { get; set; }
        public PreApprovalSnapshot PreApprovalsSnapshot { get; set; }
    }
}
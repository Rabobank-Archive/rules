using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class DeployPhaseSnapshot
    {
        public DeploymentInput DeploymentInput { get; set; }
        public string PhaseType { get; set; }
        public IEnumerable<WorkflowTask> WorkflowTasks { get; set; }
    }
}
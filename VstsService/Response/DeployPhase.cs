using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class DeployPhase
    {
        public List<WorkflowTask> WorkflowTasks { get; set; }
    }
}
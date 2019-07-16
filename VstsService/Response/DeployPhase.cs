using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class DeployPhase
    {
        public IList<WorkflowTask> WorkflowTasks { get; set; }
    }
}
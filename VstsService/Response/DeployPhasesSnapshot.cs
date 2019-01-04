using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class DeployPhasesSnapshot
    {
        public IEnumerable<WorkflowTask> WorkflowTasks { get; set; }
    }
}
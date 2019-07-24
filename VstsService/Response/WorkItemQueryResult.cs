using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class WorkItemQueryResult
    {
        public IEnumerable<WorkItem> WorkItems { get; set; }
    }
}
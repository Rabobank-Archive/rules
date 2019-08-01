using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class WorkItemQueryResult
    {
        public DateTime AsOf { get; set; }
        public IList<WorkItemReference> WorkItems { get; set; }
    }
}
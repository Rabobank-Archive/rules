using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class WorkItem : WorkItemReference
    {
        public IDictionary<string, object> Fields { get; set; }
    }
}
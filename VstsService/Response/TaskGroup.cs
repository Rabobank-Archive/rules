using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class TaskGroup
    {
        public IEnumerable<BuildStep> Tasks { get; set; }
    }
}
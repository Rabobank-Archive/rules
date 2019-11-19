using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class TaskGroupResponse
    {
        public IEnumerable<TaskGroup> Value { get; set; }
    }
}
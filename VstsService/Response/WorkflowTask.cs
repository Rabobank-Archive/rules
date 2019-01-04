using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class WorkflowTask
    {
        public Guid TaskId { get; set; }
        public Dictionary<string, string> Inputs { get; set; }
    }
}
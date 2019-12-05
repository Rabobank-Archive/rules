using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class WorkflowTask
    {
        public Guid TaskId { get; set; }
        public Dictionary<string, string> Inputs { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string DefinitionType { get; set; }
        public bool Enabled { get; set; }
    }
}
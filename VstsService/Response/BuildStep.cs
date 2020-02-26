using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class BuildStep
    {
        public bool Enabled { get; set; }
        public BuildTask Task { get; set; }
        public Dictionary<string, string> Inputs { get; set; }
    }
}
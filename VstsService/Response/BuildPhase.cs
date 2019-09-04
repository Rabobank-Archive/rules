using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class BuildPhase
    {
        public IEnumerable<BuildStep> Steps { get; set; }
    }
}
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class BuildProcess
    {
        public IEnumerable<BuildPhase> Phases { get; set; }
    }
}
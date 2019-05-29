using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class RetentionPolicy
    {
        public int DaysToKeep { get; set; }
        public int ReleasesToKeep { get; set; }
        public bool RetainBuild { get; set; }
    }
}
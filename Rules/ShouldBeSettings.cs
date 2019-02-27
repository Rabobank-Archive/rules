using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Rules.Reports;
using SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.Rules
{
    [Serializable]
    public class ShouldBeSettings
    {
        public string ProjectName { get; set; }
        public string ProjectId { get; set; }

        public IEnumerable<ApplicationGroup> GlobalPermissions { get; set; } 
            = Enumerable.Empty<ApplicationGroup>();
        
    }
}
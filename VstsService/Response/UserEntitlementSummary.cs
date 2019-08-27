using System.Collections.Generic;

namespace SecurePipelineScan.VstsService
{
    public class UserEntitlementSummary
    {
        public IEnumerable<LicenseSummary> Licenses { get; set; }
    }
}
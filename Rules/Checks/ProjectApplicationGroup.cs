using SecurePipelineScan.VstsService.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Response = SecurePipelineScan.VstsService.Response;


namespace SecurePipelineScan.Rules.Checks
{
    public static class ProjectApplicationGroup
    {
        public static bool ApplicationGroupContainsProductionEnvironmentOwner(IEnumerable<ApplicationGroup> groups)
        {
            return groups.Any(g => g.FriendlyDisplayName == "Production Environment Owners");
        }
    }
}
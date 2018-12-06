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
        public static bool HasRequiredReviewerPolicy(this Response.ApplicationGroup application, IEnumerable<ApplicationGroup> groups)
        {
            if (groups.Any(g => g.FriendlyDisplayName == "Production Environment Owners"))
            {
                return true;
            }
            return false;
        }
    }
}
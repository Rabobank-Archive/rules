using SecurePipelineScan.VstsService.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using Response = SecurePipelineScan.VstsService.Response;


namespace SecurePipelineScan.Rules.Checks
{
    public static class ProjectApplicationGroup
    {
        public static bool ApplicationGroupContainsProductionEnvironmentOwner(IEnumerable<VstsService.Response.ApplicationGroup> groups)
        {
            return groups.Any(g => g.FriendlyDisplayName == "Production Environment Owners");
        }

        public static bool ProjectAdministratorsGroupOnlyContainsRabobankProjectAdministratorsGroup(IEnumerable<VstsService.Response.ApplicationGroup> groups)
        {
            if (groups.Count() == 1)
            {
                var test = groups.First();
                if (test.FriendlyDisplayName == "Rabobank Project Administrators")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
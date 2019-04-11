using System.Collections.Generic;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Security
{
    public static class RuleSets
    {
        public static IEnumerable<IProjectRule> GlobalPermissions(IVstsRestClient client)
        {
            yield return new NobodyCanDeleteTheTeamProject(client);
        }
    }
}
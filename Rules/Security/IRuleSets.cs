using System.Collections.Generic;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Security
{
    public interface IRuleSets
    {
        IEnumerable<IProjectRule> GlobalPermissions(IVstsRestClient client);
    }
}
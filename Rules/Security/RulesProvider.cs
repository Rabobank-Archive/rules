using System.Collections.Generic;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Security
{
    public interface IRulesProvider
    {
        IEnumerable<IProjectRule> GlobalPermissions(IVstsRestClient client);
        IEnumerable<IRepositoryRule> RepositoryPermissions(IVstsRestClient client);
        
    }

    public class RulesProvider : IRulesProvider
    {
        public IEnumerable<IProjectRule> GlobalPermissions(IVstsRestClient client)
        {
            yield return new NobodyCanDeleteTheTeamProject(client);
                
        }

        public IEnumerable<IRepositoryRule> RepositoryPermissions(IVstsRestClient client)
        {
            yield return new NobodyCanDeleteTheRepository(client);
        }
    }
}
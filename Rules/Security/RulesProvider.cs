using System.Collections.Generic;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Security
{
    public interface IRulesProvider
    {
        IEnumerable<IProjectRule> GlobalPermissions(IVstsRestClient client);
        IEnumerable<IRule> RepositoryRules(IVstsRestClient client);
        IEnumerable<IRule> BuildRules(IVstsRestClient client);
        IEnumerable<IRule> ReleaseRules(IVstsRestClient client);
    }

    public class RulesProvider : IRulesProvider
    {
        public IEnumerable<IProjectRule> GlobalPermissions(IVstsRestClient client)
        {
            yield return new NobodyCanDeleteTheTeamProject(client);
        }

        public IEnumerable<IRule> RepositoryRules(IVstsRestClient client)
        {
            yield return new NobodyCanDeleteTheRepository(client);
            yield return new ReleaseBranchesProtectedByPolicies(client);
        }

        public IEnumerable<IRule> BuildRules(IVstsRestClient client)
        {
            yield return new NobodyCanDeleteBuilds(client);
        }

        public IEnumerable<IRule> ReleaseRules(IVstsRestClient client)
        {
            yield return new NobodyCanDeleteReleases(client);
            yield return new NobodyCanManageApprovalsAndCreateReleases(client);
            yield return new PipelineHasRequiredRetentionPolicy(client);
        }
    }
}
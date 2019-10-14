using System.Collections.Generic;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Security
{
    public interface IRulesProvider
    {
        IEnumerable<IProjectRule> GlobalPermissions(IVstsRestClient client);
        IEnumerable<IRepositoryRule> RepositoryRules(IVstsRestClient client);
        IEnumerable<IBuildPipelineRule> BuildRules(IVstsRestClient client);
        IEnumerable<IReleasePipelineRule> ReleaseRules(IVstsRestClient client);
    }

    public class RulesProvider : IRulesProvider
    {
        public IEnumerable<IProjectRule> GlobalPermissions(IVstsRestClient client)
        {
            yield return new NobodyCanDeleteTheTeamProject(client);
            yield return new ShouldBlockPlainTextCredentialsInPipelines(client);
        }

        public IEnumerable<IRepositoryRule> RepositoryRules(IVstsRestClient client)
        {
            yield return new NobodyCanDeleteTheRepository(client);
            yield return new ReleaseBranchesProtectedByPolicies(client);
        }

        public IEnumerable<IBuildPipelineRule> BuildRules(IVstsRestClient client)
        {
            yield return new NobodyCanDeleteBuilds(client);
            yield return new ArtifactIsStoredSecure();
            yield return new BuildPipelineHasSonarqubeTask();
            yield return new BuildPipelineHasFortifyTask();
        }

        public IEnumerable<IReleasePipelineRule> ReleaseRules(IVstsRestClient client)
        {
            yield return new NobodyCanDeleteReleases(client);
            yield return new NobodyCanManageApprovalsAndCreateReleases(client);
            yield return new PipelineHasRequiredRetentionPolicy(client);
            yield return new PipelineHasAtLeastOneStageWithApproval();
            yield return new ReleasePipelineUsesBuildArtifact();
        }
    }
}
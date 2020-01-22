using System.Collections.Generic;
using SecurePipelineScan.Rules.Security.Cmdb;
using SecurePipelineScan.Rules.Security.Cmdb.Client;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Security
{
    public interface IRulesProvider
    {
        IEnumerable<IProjectRule> GlobalPermissions(IVstsRestClient client);
        IEnumerable<IRepositoryRule> RepositoryRules(IVstsRestClient client);
        IEnumerable<IBuildPipelineRule> BuildRules(IVstsRestClient client);
        IEnumerable<IReleasePipelineRule> ReleaseRules(IVstsRestClient vstsClient, ICmdbClient cmdbClient);
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
            yield return new NobodyCanBypassPolicies(client);
        }

        public IEnumerable<IBuildPipelineRule> BuildRules(IVstsRestClient client)
        {
            yield return new NobodyCanDeleteBuilds(client);
            yield return new ArtifactIsStoredSecure(client);
            yield return new BuildPipelineHasSonarqubeTask(client);
            yield return new BuildPipelineHasFortifyTask(client);
            yield return new BuildPipelineHasNexusIqTask(client);
            yield return new BuildPipelineHasCredScanTask(client);
        }

        public IEnumerable<IReleasePipelineRule> ReleaseRules(IVstsRestClient vstsClient, ICmdbClient cmdbClient)
        {
            yield return new NobodyCanDeleteReleases(vstsClient);
            yield return new NobodyCanManageApprovalsAndCreateReleases(vstsClient);
            yield return new PipelineHasRequiredRetentionPolicy(vstsClient);
            yield return new ReleasePipelineUsesBuildArtifact();
            yield return new ProductionStageUsesArtifactFromSecureBranch(vstsClient);
            yield return new PipelineHasAtLeastOneStageWithApproval();
            yield return new ReleasePipelineHasSm9ChangeTask(vstsClient);
            yield return new ReleasePipelineHasDeploymentMethod(vstsClient, cmdbClient);
        }
    }
}
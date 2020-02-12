using Microsoft.Extensions.DependencyInjection;

namespace SecurePipelineScan.Rules.Security
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDefaultRules(this IServiceCollection collection)
        {
            return collection
                .AddGlobalPermissions()
                .AddRepositoryRules()
                .AddBuildRules()
                .AddReleaseRules();
        }

        public static IServiceCollection AddGlobalPermissions(this IServiceCollection collection) =>
            collection
                .AddSingleton<IProjectRule, NobodyCanDeleteTheTeamProject>()
                .AddSingleton<IProjectRule, ShouldBlockPlainTextCredentialsInPipelines>();

        public static IServiceCollection AddRepositoryRules(this IServiceCollection collection) =>
            collection
                .AddSingleton<IRepositoryRule, NobodyCanDeleteTheRepository>()
                .AddSingleton<IRepositoryRule, ReleaseBranchesProtectedByPolicies>()
                .AddSingleton<IRepositoryRule, NobodyCanBypassPolicies>();

        public static IServiceCollection AddBuildRules(this IServiceCollection collection) =>
            collection
                .AddSingleton<IBuildPipelineRule, NobodyCanDeleteBuilds>()
                .AddSingleton<IBuildPipelineRule, ArtifactIsStoredSecure>()
                .AddSingleton<IBuildPipelineRule, BuildPipelineHasSonarqubeTask>()
                .AddSingleton<IBuildPipelineRule, BuildPipelineHasFortifyTask>()
                .AddSingleton<IBuildPipelineRule, BuildPipelineHasNexusIqTask>()
                .AddSingleton<IBuildPipelineRule, BuildPipelineHasCredScanTask>();

        public static IServiceCollection AddReleaseRules(this IServiceCollection collection) =>
            collection
                .AddSingleton<IReleasePipelineRule, NobodyCanDeleteReleases>()
                .AddSingleton<IReleasePipelineRule, NobodyCanManageApprovalsAndCreateReleases>()
                .AddSingleton<IReleasePipelineRule, PipelineHasRequiredRetentionPolicy>()
                .AddSingleton<IReleasePipelineRule, ReleasePipelineUsesBuildArtifact>()
                .AddSingleton<IReleasePipelineRule, ProductionStageUsesArtifactFromSecureBranch>()
                .AddSingleton<IReleasePipelineRule, PipelineHasAtLeastOneStageWithApproval>()
                .AddSingleton<IReleasePipelineRule, ReleasePipelineHasSm9ChangeTask>()
                .AddSingleton<IReleasePipelineRule, ReleasePipelineHasDeploymentMethod>();
    }
}
using Microsoft.Extensions.DependencyInjection;

namespace AzureDevOps.Compliance.Rules
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
                .AddSingleton<IProjectRule, NobodyCanDeleteTheTeamProject>();

        public static IServiceCollection AddRepositoryRules(this IServiceCollection collection) =>
            collection
                .AddSingleton<IRepositoryRule, NobodyCanDeleteTheRepository>()
                .AddSingleton<IRepositoryRule, ReleaseBranchesProtectedByPolicies>()
                .AddSingleton<IRepositoryRule, NobodyCanBypassPolicies>();

        public static IServiceCollection AddBuildRules(this IServiceCollection collection) =>
            collection
                .AddSingleton<IBuildPipelineRule, NobodyCanDeleteBuilds>();

        public static IServiceCollection AddReleaseRules(this IServiceCollection collection) =>
            collection
                .AddSingleton<IReleasePipelineRule, NobodyCanDeleteReleases>()
                .AddSingleton<IReleasePipelineRule, PipelineHasRequiredRetentionPolicy>();
    }
}
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class ServiceCollectionTests
    {
        [Fact]
        public void GlobalPermissions()
        {
            var service = new ServiceCollection()
                .AddDefaultRules()
                .AddSingleton(Substitute.For<IVstsRestClient>())
                .BuildServiceProvider();

            var rules = service.GetServices<IProjectRule>();
            rules.OfType<NobodyCanDeleteTheTeamProject>().ShouldNotBeEmpty();
        }

        [Fact]
        public void RepositoryRules()
        {
            var service = new ServiceCollection()
                .AddDefaultRules()
                .AddSingleton(Substitute.For<IVstsRestClient>())
                .AddSingleton(Substitute.For<IPoliciesResolver>())
                .BuildServiceProvider();

            var rules = service.GetServices<IRepositoryRule>().ToList();
            rules
                .OfType<NobodyCanDeleteTheRepository>()
                .ShouldNotBeEmpty();

            rules
                .OfType<ReleaseBranchesProtectedByPolicies>()
                .ShouldNotBeEmpty();
        }

        [Fact]
        public void BuildRules()
        {
            var service = new ServiceCollection()
                .AddDefaultRules()
                .AddSingleton(Substitute.For<IVstsRestClient>())
                .BuildServiceProvider();

            var rules = service.GetServices<IBuildPipelineRule>().ToList();
            rules
                .OfType<NobodyCanDeleteBuilds>()
                .ShouldNotBeEmpty();
        }

        [Fact]
        public void ReleaseRules()
        {
            var service = new ServiceCollection()
                .AddDefaultRules()
                .AddSingleton(Substitute.For<IVstsRestClient>())
                .AddSingleton(Substitute.For<IProductionItemsResolver>())
                .BuildServiceProvider();

            var rules = service.GetServices<IReleasePipelineRule>().ToList();
            rules
                .OfType<NobodyCanDeleteReleases>()
                .ShouldNotBeEmpty();
        }

        [Fact]
        public void AllRulesShouldBeInProvider()
        {
            var service = new ServiceCollection()
                .AddDefaultRules();

            var types = typeof(IRule).Assembly.GetTypes().Where(t => typeof(IRule).IsAssignableFrom(t) && !t.IsInterface).ToList();
            types.ShouldNotBeEmpty();

            types.ShouldAllBe(t => service.Select(r => r.ImplementationType).Contains(t));
        }
    }
}
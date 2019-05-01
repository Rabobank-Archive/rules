using System.Linq;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class RulesProviderTests
    {
        [Fact]
        public void GlobalPermissions()
        {
            var rules = new RulesProvider().GlobalPermissions(Substitute.For<IVstsRestClient>());
            rules.OfType<NobodyCanDeleteTheTeamProject>().ShouldNotBeEmpty();
            
        }

        [Fact]
        public void RepositoryPermissions()
        {
            var fixture = new Fixture();
            fixture.Customize<SecurityNamespace>(ctx =>
                ctx.With(x => x.DisplayName, "Git Repositories"));
            
            var client = new FixtureClient(fixture);
            var rules = new RulesProvider().RepositoryRules(client);
            
            rules
                .OfType<NobodyCanDeleteTheRepository>()
                .ShouldNotBeEmpty();
            
            rules
                .OfType<ReleaseBranchesProtectedByPolicies>()
                .ShouldNotBeEmpty();
        }
    }
}
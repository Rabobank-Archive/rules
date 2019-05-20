using System.Linq;
using AutoFixture;
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
            var rules = new RulesProvider().GlobalPermissions(null);
            rules.OfType<NobodyCanDeleteTheTeamProject>().ShouldNotBeEmpty();
        }

        [Fact]
        public void RepositoryRules()
        {
            var rules = new RulesProvider().RepositoryRules(null);
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
            var rules = new RulesProvider().BuildRules(null);
            rules
                .OfType<NobodyCanDeleteBuilds>()
                .ShouldNotBeEmpty();
        }

        [Fact]
        public void ReleaseRules()
        {
            var rules = new RulesProvider().ReleaseRules(null);
            rules
                .OfType<NobodyCanDeleteReleases>()
                .ShouldNotBeEmpty();
        }
    }
}
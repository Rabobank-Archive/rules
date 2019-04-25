using System.Linq;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
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
            var repoRules = new RulesProvider().RepositoryRules(Substitute.For<IVstsRestClient>());
            repoRules.OfType<NobodyCanDeleteTheRepository>().ShouldNotBeEmpty();
            repoRules.OfType<MasterReleaseBranchesProtectedWith4Eyes>().ShouldNotBeEmpty();
        }
    }
}
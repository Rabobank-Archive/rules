using System.Linq;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class RuleSetsTests
    {
        [Fact]
        public void GetPermissions()
        {
            var rules = new RulesProvider().GlobalPermissions(Substitute.For<IVstsRestClient>());
            rules.OfType<NobodyCanDeleteTheTeamProject>().ShouldNotBeEmpty();
            
            var repoRules = new RulesProvider().RepositoryPermissions(Substitute.For<IVstsRestClient>());
            repoRules.OfType<NobodyCanDeleteTheRepository>().ShouldNotBeEmpty();
        }
    }
}
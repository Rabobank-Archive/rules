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
        public void GlobalPermissions()
        {
            var rules = new RulesProvider().GlobalPermissions(Substitute.For<IVstsRestClient>());
            rules.OfType<NobodyCanDeleteTheTeamProject>().ShouldNotBeEmpty();
        }
    }
}
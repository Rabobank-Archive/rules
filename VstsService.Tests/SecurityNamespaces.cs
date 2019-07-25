using Shouldly;
using System.Linq;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("Category","integration")]
    public class SecurityNamespaces : IClassFixture<TestConfig>
    {
        private readonly IVstsRestClient _client;

        public SecurityNamespaces(TestConfig config)
        {
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryNamespaces()
        {
            var target = _client.Get(Requests.SecurityNamespace.SecurityNamespaces()).ToList();
            target.ShouldNotBeEmpty();

            var first = target.First();
            first.Actions.ShouldNotBeEmpty();

            var action = first.Actions.First();
            action.Name.ShouldNotBeEmpty();
            action.DisplayName.ShouldNotBeEmpty();
            action.Bit.ShouldNotBe(0);
        }
    }
}
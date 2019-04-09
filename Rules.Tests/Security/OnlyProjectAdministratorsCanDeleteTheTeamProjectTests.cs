using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests
{
    public class OnlyProjectAdministratorsCanDeleteTheTeamProjectTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private VstsRestClient _client;

        public OnlyProjectAdministratorsCanDeleteTheTeamProjectTests(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
        }
        
        [Fact]
        public void Test()
        {
            var rule = new OnlyProjectAdministratorsCanDeleteTheTeamProject(_client);
            rule.Evaluate(_config.Project).ShouldBeTrue();
        }
    }
}
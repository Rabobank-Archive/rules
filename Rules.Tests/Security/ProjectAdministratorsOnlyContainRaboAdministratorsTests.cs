
using System.Linq;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests
{
    public class ProjectAdministratorsOnlyContainRaboAdministratorsTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private VstsRestClient _client;

        public ProjectAdministratorsOnlyContainRaboAdministratorsTests(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
        }
        
        [Fact]
        public void Test()
        {           
           var rule = new ProjectAdministratorsOnlyContainRaboAdministrators(_client);
           rule.Evaluate(_config.Project).ShouldBeTrue();
        }
    }
}
using System.Threading.Tasks;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class ArtifactIsStoredSecureTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public ArtifactIsStoredSecureTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        public async Task EvaluateBuildIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(VstsService.Requests.Project.Properties(_config.Project))).Id;

            var rule = new ArtifactIsStoredSecure(client);
            (await rule.EvaluateAsync(projectId, "2")).ShouldBeTrue();
        }
    }
}
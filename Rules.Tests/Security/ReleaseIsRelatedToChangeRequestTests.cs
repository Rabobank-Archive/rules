using SecurePipelineScan.VstsService;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class ReleaseIsRelatedToChangeRequestTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        [Fact]
        public void IntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;
            
            
        }
    }
}
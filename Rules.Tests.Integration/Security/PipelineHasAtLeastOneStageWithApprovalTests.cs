using System.Threading.Tasks;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;

namespace Rules.Tests.Integration.Security
{
    public class PipelineHasAtLeastOneStageWithApprovalTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public PipelineHasAtLeastOneStageWithApprovalTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateIntegrationTest()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, "1"))
                .ConfigureAwait(false);

            //Act
            var rule = new PipelineHasAtLeastOneStageWithApproval();
            (await rule.EvaluateAsync(_config.Project, releasePipeline)).ShouldBe(true);
        }
    }
}
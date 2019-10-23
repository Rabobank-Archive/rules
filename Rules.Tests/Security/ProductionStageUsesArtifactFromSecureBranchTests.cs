using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using System.Threading.Tasks;
using Xunit;
using SecurePipelineScan.Rules.Security;
using System;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class ProductionStageUsesArtifactFromSecureBranchTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private const string PipelineId = "1";

        public ProductionStageUsesArtifactFromSecureBranchTests(TestConfig config) => _config = config;

        [Fact]
        public async Task EvaluateIntegrationTest()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, PipelineId))
                .ConfigureAwait(false);

            //Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch();
            var result = await rule.EvaluateAsync(_config.Project, _config.stageId, releasePipeline);

            //Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task StageHasNoPreferedBranchFilter()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, PipelineId))
                .ConfigureAwait(false);

            var stageId = "1";

            //Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch();
            var result = await rule.EvaluateAsync(_config.Project, stageId, releasePipeline);

            //Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task NoStageIdProved()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, PipelineId))
                .ConfigureAwait(false);

            //Act & Assert
            var rule = new ProductionStageUsesArtifactFromSecureBranch();
            await Assert.ThrowsAsync<ArgumentNullException>(() => rule.EvaluateAsync(_config.Project, null, releasePipeline));
        }

        [Fact]
        public async Task NoReleasePipelineProved()
        {
            //Act & Assert
            var rule = new ProductionStageUsesArtifactFromSecureBranch();
            await Assert.ThrowsAsync<ArgumentNullException>(() => rule.EvaluateAsync(_config.Project, _config.stageId, null));
        }

    }
}

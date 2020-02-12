using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using System.Threading.Tasks;
using Xunit;
using SecurePipelineScan.Rules.Security;
using AutoFixture;
using NSubstitute;
using Response = SecurePipelineScan.VstsService.Response;
using System.Collections.Generic;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class ReleasePipelineUsesBuildArtifactTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private const string PipelineId = "1";

        public ReleasePipelineUsesBuildArtifactTests(TestConfig config) => _config = config;

        [Fact]
        public async Task EvaluateIntegrationTest()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, PipelineId))
                .ConfigureAwait(false);

            //Act
            var rule = new ReleasePipelineUsesBuildArtifact();
            var result = await rule.EvaluateAsync(_config.Project, releasePipeline);

            //Assert
            result.ShouldBe(true);
        }

        [Fact]
        public async Task ReturnFalseForReleasePipelineWithoutArtifacts()
        {
            //Arrange
            var fixture = new Fixture();
            var releasePipeline = fixture.Create<Response.ReleaseDefinition>();
            releasePipeline.Artifacts = new List<Response.Artifact>();

            //Act
            var rule = new ReleasePipelineUsesBuildArtifact();
            var result = await rule.EvaluateAsync(_config.Project, releasePipeline);

            //Assert
            result.ShouldBe(false);
        }
    }
}
using AutoFixture;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class ReleasePipelineHasDeploymentMethodTests : IClassFixture<TestConfig>
    {
        [Fact]
        public async Task WhenReleasePipelineHasNoDeploymentMethodsThenShouldReturnNull()
        {
            //Arrange
            var fixture = new Fixture();
            var releasePipeline = fixture.Create<Response.ReleaseDefinition>();

            //Act
            var rule = new ReleasePipelineHasDeploymentMethod();
            var result = await rule.EvaluateAsync("TAS", null, releasePipeline);

            //Assert
            result.ShouldBe(null);
        }

        [Fact]
        public async Task WhenReleasePipelineHasDeploymentMethodsThenShouldReturnTrue()
        {
            //Arrange
            var fixture = new Fixture();
            var releasePipeline = fixture.Create<Response.ReleaseDefinition>();

            //Act
            var rule = new ReleasePipelineHasDeploymentMethod();
            var result = await rule.EvaluateAsync("TAS", releasePipeline.Environments.First().Id, releasePipeline);

            //Assert
            result.ShouldBe(true);
        }

        [Fact]
        public async Task WhenReleasePipelineHasNoValidDeploymentMethodsThenShouldReturnFalse()
        {
            //Arrange
            var fixture = new Fixture();
            var releasePipeline = fixture.Create<Response.ReleaseDefinition>();

            //Act
            var rule = new ReleasePipelineHasDeploymentMethod();
            var result = await rule.EvaluateAsync("TAS", "???", releasePipeline);

            //Assert
            result.ShouldBe(false);
        }
    }
}
using AutoFixture;
using System.Linq;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using NSubstitute.ReturnsExtensions;
using SecurePipelineScan.Rules.Security;
using Xunit;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class ReleasePipelineHasDeploymentMethodTests : IClassFixture<TestConfig>
    {
        [Fact]
        public async Task WhenReleasePipelineHasNoDeploymentMethodsThenShouldReturnFalse()
        {
            //Arrange
            var fixture = new Fixture();
            var releasePipeline = fixture.Create<Response.ReleaseDefinition>();

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync("TAS", releasePipeline.Id).ReturnsNull();

            //Act
            var rule = new ReleasePipelineHasDeploymentMethod(productionItems);
            var result = await rule.EvaluateAsync("TAS", releasePipeline);

            //Assert
            result.ShouldBe(false);
        }

        [Fact]
        public async Task WhenReleasePipelineHasDeploymentMethodsThenShouldReturnTrue()
        {
            //Arrange
            var fixture = new Fixture();
            var releasePipeline = fixture.Create<Response.ReleaseDefinition>();

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync("TAS", releasePipeline.Id).Returns((new[] { releasePipeline.Environments.First().Id }));

            //Act
            var rule = new ReleasePipelineHasDeploymentMethod(productionItems);
            var result = await rule.EvaluateAsync("TAS", releasePipeline);

            //Assert
            result.ShouldBe(true);
        }

        [Fact]
        public async Task WhenReleasePipelineHasNoValidDeploymentMethodsThenShouldReturnFalse()
        {
            //Arrange
            var fixture = new Fixture();
            var releasePipeline = fixture.Create<Response.ReleaseDefinition>();

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync("TAS", releasePipeline.Id).Returns(fixture.CreateMany<string>());

            //Act
            var rule = new ReleasePipelineHasDeploymentMethod(productionItems);
            var result = await rule.EvaluateAsync("TAS", releasePipeline);

            //Assert
            result.ShouldBe(false);
        }

        [Fact]
        public async Task WhenNonProdReleasePipelineThenShouldReturnTrue()
        {
            //Arrange
            var fixture = new Fixture();
            var releasePipeline = fixture.Create<Response.ReleaseDefinition>();
            var pi = Substitute.For<IProductionItemsResolver>();
            pi.ResolveAsync("TAS", releasePipeline.Id).Returns(new[] { "NON-PROD" });

            //Act
            var rule = new ReleasePipelineHasDeploymentMethod(pi);
            var result = await rule.EvaluateAsync("TAS", releasePipeline);

            //Assert
            result.ShouldBe(true);
        }
    }
}
using System;
using System.Threading.Tasks;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;

namespace Rules.Tests.Integration.Security
{
    public class ProductionStageUsesArtifactFromSecureBranchTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private const string PipelineId = "1";

        public ProductionStageUsesArtifactFromSecureBranchTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateIntegrationTest()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, PipelineId))
                .ConfigureAwait(false);

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync(_config.Project, releasePipeline.Id).Returns(new[] {_config.StageId});

            //Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(client, productionItems);
            var result = await rule.EvaluateAsync(_config.Project, releasePipeline);

            //Assert
            Assert.Equal(true, result);
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task StageHasNoPreferedBranchFilter()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, PipelineId))
                .ConfigureAwait(false);

            const string stageId = "1";
            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync(_config.Project, releasePipeline.Id).Returns(new[] {stageId});

            //Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(client, productionItems);
            var result = await rule.EvaluateAsync(_config.Project, releasePipeline);

            //Assert
            Assert.Equal(false, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        [Trait("category", "integration")]
        public async Task NoStageIdProved(string stageId)
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, PipelineId))
                .ConfigureAwait(false);

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync(_config.Project, Arg.Any<string>()).Returns(new[] {stageId});

            //Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(client, productionItems);
            var result = await rule.EvaluateAsync(_config.Project, releasePipeline);

            //Assert
            result.ShouldBeNull();
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task NoReleasePipelineProved()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);

            //Act & Assert
            var rule = new ProductionStageUsesArtifactFromSecureBranch(client,
                Substitute.For<IProductionItemsResolver>());
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                rule.EvaluateAsync(_config.Project, null));
        }

        [Fact(Skip = "For manual execution only")]
        [Trait("category", "integration")]
        public async Task Reconcile()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var rule = (IReconcile) new ProductionStageUsesArtifactFromSecureBranch(client,
                Substitute.For<IProductionItemsResolver>());
            await rule.ReconcileAsync(_config.Project, "1").ConfigureAwait(false);
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Tests.Security
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
        public async Task EvaluateIntegrationTest()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, PipelineId))
                .ConfigureAwait(false);

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync(_config.Project, releasePipeline.Id).Returns(new[] { _config.StageId });

            //Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(client, productionItems);
            var result = await rule.EvaluateAsync(_config.Project, releasePipeline);

            //Assert
            Assert.Equal(true, result);
        }

        [Fact]
        public async Task StageHasNoPreferedBranchFilter()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, PipelineId))
                .ConfigureAwait(false);

            const string stageId = "1";
            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync(_config.Project, releasePipeline.Id).Returns(new[] { stageId });

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
        public async Task NoStageIdProved(string stageId)
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, PipelineId))
                .ConfigureAwait(false);

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync(_config.Project, Arg.Any<string>()).Returns(new[] { stageId });

            //Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(client, productionItems);
            var result = await rule.EvaluateAsync(_config.Project, releasePipeline);

            //Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task NoReleasePipelineProved()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);

            //Act & Assert
            var rule = new ProductionStageUsesArtifactFromSecureBranch(client, Substitute.For<IProductionItemsResolver>());
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                rule.EvaluateAsync(_config.Project, null));
        }

        [Fact]
        public async Task ReconcileAsync_WithBuildArtifactAndEmptyConditions_OneConditionShouldBeAdded()
        {
            // arrange
            var definition = JObject.FromObject(new
            {
                environments = new[]
                {
                    new
                    {
                        id = 1,
                        conditions = new object[0]
                    }
                },
                artifacts = new[]
                {
                    new
                    {
                        type = "Build",
                        alias = "artifactAlias1"
                    }
                }
            });

            var client = Substitute.For<IVstsRestClient>();
            client
                .GetAsync(Arg.Any<IVstsRequest<JObject>>())
                .Returns(definition);

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync(_config.Project, "1").Returns(new[] { "1" });

            var rule = (IReconcile)new ProductionStageUsesArtifactFromSecureBranch(client, productionItems);

            // act
            await rule.ReconcileAsync(_config.Project, "1");

            // assert
            await client
                .Received(1)
                .PutAsync(Arg.Any<VsrmRequest<object>>(), Arg.Is<JObject>(
                    d => d.SelectTokens("environments[*].conditions" +
                                        "[?(@.name == 'artifactAlias1' && @.conditionType == 'artifact')]"
                    ).Count() == 1));
        }

        [Fact]
        public async Task EvaluateAsync_ArtifactWithBranchFilter()
        {
            // Arrange
            var releasePipeline = new Response.ReleaseDefinition
            {
                Id = "1",
                Artifacts = new[]
                {
                    new Response.Artifact
                    {
                        Alias = "function",
                        Type = "Build"
                    }
                },
                Environments = new[]
                {
                    new Response.ReleaseDefinitionEnvironment
                    {
                        Id = "1",
                        Conditions = new[]
                        {
                            new Response.Condition
                            {
                                ConditionType = "artifact",
                                Name = "function",
                                Value = "{\"sourceBranch\":\"master\",\"tags\":[],\"useBuildDefinitionBranch\":false,\"createReleaseOnBuildTagging\":false}"
                            },
                            new Response.Condition
                            {
                                ConditionType = "artifact",
                                Name = "function",
                                Value = "{\"sourceBranch\":\"-fghjk\",\"tags\":[],\"useBuildDefinitionBranch\":false,\"createReleaseOnBuildTagging\":false}"
                            }
                        }
                    }
                }
            };

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync(_config.Project, "1").Returns(new[] { "1" });

            // Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>(), productionItems);
            var result = await rule.EvaluateAsync(_config.Project, releasePipeline);

            // Assert

            result.ShouldBe(true);
        }

        [Fact]
        public async Task EvaluateAsync_MultipleArtifactsNotAllBranchFilters()
        {
            // Arrange
            var releasePipeline = new Response.ReleaseDefinition()
            {
                Id = "1",
                Artifacts = new[]
                {
                    new Response.Artifact
                    {
                        Alias = "function",
                        Type = "Build"
                    },
                     new Response.Artifact
                    {
                        Alias = "infra",
                        Type = "Build"
                    }
                },
                Environments = new[]
                {
                    new Response.ReleaseDefinitionEnvironment
                    {
                        Id = "1",
                        Conditions = new[]
                        {
                            new Response.Condition
                            {
                                ConditionType = "artifact",
                                Name = "function",
                                Value = "{\"sourceBranch\":\"master\",\"tags\":[],\"useBuildDefinitionBranch\":false,\"createReleaseOnBuildTagging\":false}"
                            }
                        }
                    }
                }
            };

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync(_config.Project, "1").Returns(new[] { "1" });

            // Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>(), productionItems);
            var result = await rule.EvaluateAsync(_config.Project, releasePipeline);

            // Assert

            result.ShouldBe(false);
        }

        [Fact]
        public async Task EvaluateAsync_ConditionTypeNotArtifact()
        {
            // Arrange
            var releasePipeline = new Response.ReleaseDefinition()
            {
                Id = "1",
                Artifacts = new[]
                {
                    new Response.Artifact
                    {
                        Alias = "function",
                        Type = "Build"
                    }
                },
                Environments = new[]
                {
                    new Response.ReleaseDefinitionEnvironment
                    {
                        Id = "1",
                        Conditions = new[]
                        {
                            new Response.Condition
                            {
                                ConditionType = "asdfg",
                                Name = "function",
                                Value = "{\"sourceBranch\":\"master\",\"tags\":[],\"useBuildDefinitionBranch\":false,\"createReleaseOnBuildTagging\":false}"
                            }
                        }
                    }
                }
            };

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync(_config.Project, "1").Returns(new[] { "1" });

            // Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>(), productionItems);
            var result = await rule.EvaluateAsync(_config.Project, releasePipeline);

            // Assert

            result.ShouldBe(false);
        }

        [Fact]
        public async Task EvaluateAsync_StageIdAreNotEqual()
        {
            // Arrange
            var releasePipeline = new Response.ReleaseDefinition()
            {
                Artifacts = new[]
                {
                    new Response.Artifact
                    {
                        Alias = "function",
                        Type = "Build"
                    }
                },
                Environments = new[]
                {
                    new Response.ReleaseDefinitionEnvironment
                    {
                        Id = "1"
                    }
                }
            };

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync(_config.Project, Arg.Any<string>()).Returns(new[] { "2" });

            // Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>(), productionItems);
            var result = await rule.EvaluateAsync(_config.Project, releasePipeline);

            // Assert

            result.ShouldBe(null);
        }

        [Fact]
        public async Task EvaluateAsync_ConditionSourceBranchNotMaster()
        {
            // Arrange
            var releasePipeline = new Response.ReleaseDefinition()
            {
                Id = "1",
                Artifacts = new[]
                {
                    new Response.Artifact
                    {
                        Alias = "function",
                        Type = "Build"
                    }
                },
                Environments = new[]
                {
                    new Response.ReleaseDefinitionEnvironment
                    {
                        Id = "1",
                        Conditions = new[]
                        {
                            new Response.Condition
                            {
                                ConditionType = "artifact",
                                Name = "function",
                                Value = "{\"sourceBranch\":\"test\",\"tags\":[],\"useBuildDefinitionBranch\":false,\"createReleaseOnBuildTagging\":false}"
                            }
                        }
                    }
                }
            };

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync(_config.Project, "1").Returns(new[] { "1" });

            // Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>(), productionItems);
            var result = await rule.EvaluateAsync(_config.Project, releasePipeline);

            // Assert

            result.ShouldBe(false);
        }

        [Fact]
        public async Task EvaluateAsync_OtherArtifactType()
        {
            // Arrange
            var releasePipeline = new Response.ReleaseDefinition()
            {
                Artifacts = new[]
                {
                    new Response.Artifact
                    {
                        Alias = "function",
                        Type = "other"
                    }
                }
            };

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync(_config.Project, Arg.Any<string>()).Returns(new[] { "1" });

            // Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>(), productionItems);
            var result = await rule.EvaluateAsync(_config.Project, releasePipeline);

            // Assert

            result.ShouldBe(null);
        }

        [Fact(Skip = "For manual execution only")]
        public async Task Reconcile()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var rule = (IReconcile)new ProductionStageUsesArtifactFromSecureBranch(client, Substitute.For<IProductionItemsResolver>());
            await rule.ReconcileAsync(_config.Project, "1").ConfigureAwait(false);
        }
    }
}
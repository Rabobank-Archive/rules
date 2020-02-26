using System.Linq;
using Newtonsoft.Json.Linq;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class ProductionStageUsesArtifactFromSecureBranchTests
    {
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
            productionItems.ResolveAsync("", "1").Returns(new[] {"1"});

            var rule = (IReconcile) new ProductionStageUsesArtifactFromSecureBranch(client, productionItems);

            // act
            await rule.ReconcileAsync("", "1");

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
            var releasePipeline = new ReleaseDefinition
            {
                Id = "1",
                Artifacts = new[]
                {
                    new Artifact
                    {
                        Alias = "function",
                        Type = "Build"
                    }
                },
                Environments = new[]
                {
                    new ReleaseDefinitionEnvironment
                    {
                        Id = "1",
                        Conditions = new[]
                        {
                            new Condition
                            {
                                ConditionType = "artifact",
                                Name = "function",
                                Value =
                                    "{\"sourceBranch\":\"master\",\"tags\":[],\"useBuildDefinitionBranch\":false,\"createReleaseOnBuildTagging\":false}"
                            },
                            new Condition
                            {
                                ConditionType = "artifact",
                                Name = "function",
                                Value =
                                    "{\"sourceBranch\":\"-fghjk\",\"tags\":[],\"useBuildDefinitionBranch\":false,\"createReleaseOnBuildTagging\":false}"
                            }
                        }
                    }
                }
            };

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync("", "1").Returns(new[] {"1"});

            // Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>(),
                productionItems);
            var result = await rule.EvaluateAsync("", releasePipeline);

            // Assert

            result.ShouldBe(true);
        }

        [Fact]
        public async Task EvaluateAsync_MultipleArtifactsNotAllBranchFilters()
        {
            // Arrange
            var releasePipeline = new ReleaseDefinition
            {
                Id = "1",
                Artifacts = new[]
                {
                    new Artifact
                    {
                        Alias = "function",
                        Type = "Build"
                    },
                    new Artifact
                    {
                        Alias = "infra",
                        Type = "Build"
                    }
                },
                Environments = new[]
                {
                    new ReleaseDefinitionEnvironment
                    {
                        Id = "1",
                        Conditions = new[]
                        {
                            new Condition
                            {
                                ConditionType = "artifact",
                                Name = "function",
                                Value =
                                    "{\"sourceBranch\":\"master\",\"tags\":[],\"useBuildDefinitionBranch\":false,\"createReleaseOnBuildTagging\":false}"
                            }
                        }
                    }
                }
            };

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync("", "1").Returns(new[] {"1"});

            // Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>(),
                productionItems);
            var result = await rule.EvaluateAsync("", releasePipeline);

            // Assert

            result.ShouldBe(false);
        }

        [Fact]
        public async Task EvaluateAsync_ConditionTypeNotArtifact()
        {
            // Arrange
            var releasePipeline = new ReleaseDefinition
            {
                Id = "1",
                Artifacts = new[]
                {
                    new Artifact
                    {
                        Alias = "function",
                        Type = "Build"
                    }
                },
                Environments = new[]
                {
                    new ReleaseDefinitionEnvironment
                    {
                        Id = "1",
                        Conditions = new[]
                        {
                            new Condition
                            {
                                ConditionType = "asdfg",
                                Name = "function",
                                Value =
                                    "{\"sourceBranch\":\"master\",\"tags\":[],\"useBuildDefinitionBranch\":false,\"createReleaseOnBuildTagging\":false}"
                            }
                        }
                    }
                }
            };

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync("", "1").Returns(new[] {"1"});

            // Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>(),
                productionItems);
            var result = await rule.EvaluateAsync("", releasePipeline);

            // Assert

            result.ShouldBe(false);
        }

        [Fact]
        public async Task EvaluateAsync_StageIdAreNotEqual()
        {
            // Arrange
            var releasePipeline = new ReleaseDefinition
            {
                Artifacts = new[]
                {
                    new Artifact
                    {
                        Alias = "function",
                        Type = "Build"
                    }
                },
                Environments = new[]
                {
                    new ReleaseDefinitionEnvironment
                    {
                        Id = "1"
                    }
                }
            };

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync("", Arg.Any<string>()).Returns(new[] {"2"});

            // Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>(),
                productionItems);
            var result = await rule.EvaluateAsync("", releasePipeline);

            // Assert

            result.ShouldBe(null);
        }

        [Fact]
        public async Task EvaluateAsync_ConditionSourceBranchNotMaster()
        {
            // Arrange
            var releasePipeline = new ReleaseDefinition
            {
                Id = "1",
                Artifacts = new[]
                {
                    new Artifact
                    {
                        Alias = "function",
                        Type = "Build"
                    }
                },
                Environments = new[]
                {
                    new ReleaseDefinitionEnvironment
                    {
                        Id = "1",
                        Conditions = new[]
                        {
                            new Condition
                            {
                                ConditionType = "artifact",
                                Name = "function",
                                Value =
                                    "{\"sourceBranch\":\"test\",\"tags\":[],\"useBuildDefinitionBranch\":false,\"createReleaseOnBuildTagging\":false}"
                            }
                        }
                    }
                }
            };

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync("", "1").Returns(new[] {"1"});

            // Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>(),
                productionItems);
            var result = await rule.EvaluateAsync("", releasePipeline);

            // Assert

            result.ShouldBe(false);
        }

        [Fact]
        public async Task EvaluateAsync_OtherArtifactType()
        {
            // Arrange
            var releasePipeline = new ReleaseDefinition
            {
                Artifacts = new[]
                {
                    new Artifact
                    {
                        Alias = "function",
                        Type = "other"
                    }
                }
            };

            var productionItems = Substitute.For<IProductionItemsResolver>();
            productionItems.ResolveAsync("", Arg.Any<string>()).Returns(new[] {"1"});

            // Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>(),
                productionItems);
            var result = await rule.EvaluateAsync("", releasePipeline);

            // Assert

            result.ShouldBe(null);
        }
    }
}
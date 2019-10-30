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

            //Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(client);
            var result = await rule.EvaluateAsync(_config.Project, _config.stageId, releasePipeline);

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

            //Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(client);
            var result = await rule.EvaluateAsync(_config.Project, stageId, releasePipeline);

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

            //Act
            var rule = new ProductionStageUsesArtifactFromSecureBranch(client);
            var result = await rule.EvaluateAsync(_config.Project, stageId, releasePipeline);

            //Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task NoReleasePipelineProved()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);

            //Act & Assert
            var rule = new ProductionStageUsesArtifactFromSecureBranch(client);
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                rule.EvaluateAsync(_config.Project, _config.stageId, null));
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

            var rule = (IReconcile) new ProductionStageUsesArtifactFromSecureBranch(client);

            // act
            await rule.ReconcileAsync("projectId", "1", "1");

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

            // Act

            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>());
            var result = await rule.EvaluateAsync("1", "1", releasePipeline);

            // Assert

            result.ShouldBe(true);
        }

        [Fact]
        public async Task EvaluateAsync_MultipleArtifactsNotAllBranchFilters()
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

            // Act

            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>());
            var result = await rule.EvaluateAsync("1", "1", releasePipeline);

            // Assert

            result.ShouldBe(false);
        }

        [Fact]
        public async Task EvaluateAsync_ConditionTypeNotArtifact()
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

            // Act

            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>());
            var result = await rule.EvaluateAsync("1", "1", releasePipeline);

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

            // Act

            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>());
            var result = await rule.EvaluateAsync("1", "2", releasePipeline);

            // Assert

            result.ShouldBe(null);
        }

        [Fact]
        public async Task EvaluateAsync_ConditionSourceBranchNotMaster()
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

            // Act

            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>());
            var result = await rule.EvaluateAsync("1", "1", releasePipeline);

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

            // Act

            var rule = new ProductionStageUsesArtifactFromSecureBranch(Substitute.For<IVstsRestClient>());
            var result = await rule.EvaluateAsync("1", "1", releasePipeline);

            // Assert

            result.ShouldBe(null);
        }

        [Fact(Skip = "For manual execution only")]
        public async Task Reconcile()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var rule = (IReconcile)new ProductionStageUsesArtifactFromSecureBranch(client);
            await rule.ReconcileAsync(_config.Project, "2", "1").ConfigureAwait(false);
        }
    }
}
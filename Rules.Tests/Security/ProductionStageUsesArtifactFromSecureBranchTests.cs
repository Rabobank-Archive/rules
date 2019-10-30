using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using System.Threading.Tasks;
using Xunit;
using SecurePipelineScan.Rules.Security;
using System;
using Response = SecurePipelineScan.VstsService.Response;

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
            Assert.Equal(true, result);
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
            var rule = new ProductionStageUsesArtifactFromSecureBranch();
            var result = await rule.EvaluateAsync(_config.Project, stageId, releasePipeline);

            //Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task NoReleasePipelineProved()
        {
            //Act & Assert
            var rule = new ProductionStageUsesArtifactFromSecureBranch();
            await Assert.ThrowsAsync<ArgumentNullException>(() => rule.EvaluateAsync(_config.Project, _config.stageId, null));
        }

        [Fact]
        public async Task ReleasePipeline_ArtifactWithBranchFilter()
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

            var rule = new ProductionStageUsesArtifactFromSecureBranch();
            var result = await rule.EvaluateAsync("1", "1", releasePipeline);

            // Assert

            result.ShouldBe(true);
        }

        [Fact]
        public async Task ReleasePipeline_MultipleArtifactsNotAllBranchFilters()
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

            var rule = new ProductionStageUsesArtifactFromSecureBranch();
            var result = await rule.EvaluateAsync("1", "1", releasePipeline);

            // Assert

            result.ShouldBe(false);
        }

        [Fact]
        public async Task ReleasePipeline_ConditionNotSet()
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

            var rule = new ProductionStageUsesArtifactFromSecureBranch();
            var result = await rule.EvaluateAsync("1", "1", releasePipeline);

            // Assert

            result.ShouldBe(false);
        }

        [Fact]
        public async Task ReleasePipeline_StageIdAreNotEqual()
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

            var rule = new ProductionStageUsesArtifactFromSecureBranch();
            var result = await rule.EvaluateAsync("1", "2", releasePipeline);

            // Assert

            result.ShouldBe(null);
        }

        [Fact]
        public async Task ReleasePipeline_ConditionSetNotMaster()
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

            var rule = new ProductionStageUsesArtifactFromSecureBranch();
            var result = await rule.EvaluateAsync("1", "1", releasePipeline);

            // Assert

            result.ShouldBe(false);
        }

        [Fact]
        public async Task ReleasePipeline_OtherArtifactType()
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

            var rule = new ProductionStageUsesArtifactFromSecureBranch();
            var result = await rule.EvaluateAsync("1", "1", releasePipeline);

            // Assert

            result.ShouldBe(null);
        }
    }
}

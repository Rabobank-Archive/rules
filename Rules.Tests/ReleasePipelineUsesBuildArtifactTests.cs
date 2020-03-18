using System.Collections.Generic;
using AutoFixture;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace AzureDevOps.Compliancy.Rules.Tests
{
    public class ReleasePipelineUsesBuildArtifactTests
    {
        [Fact]
        public async Task ReturnFalseForReleasePipelineWithoutArtifacts()
        {
            //Arrange
            var fixture = new Fixture();
            var releasePipeline = fixture.Create<ReleaseDefinition>();
            releasePipeline.Artifacts = new List<Artifact>();

            //Act
            var rule = new ReleasePipelineUsesBuildArtifact();
            var result = await rule.EvaluateAsync(fixture.Create<string>(), releasePipeline);

            //Assert
            result.ShouldBe(false);
        }
    }
}
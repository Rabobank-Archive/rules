using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Newtonsoft.Json.Linq;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using VstsService.Response;
using Xunit;
using static SecurePipelineScan.VstsService.Requests.YamlPipeline;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class BuildPipelineHasCredScanTaskTests
    {
        private readonly Fixture _fixture = new Fixture { RepeatCount = 1 };

        public BuildPipelineHasCredScanTaskTests()
        {
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        public void BuildPipelineHasCredScanTask_ShouldHaveCorrectProperties()
        {
            // Arrange
            var client = Substitute.For<IVstsRestClient>();
            var rule = new BuildPipelineHasCredScanTask(client);

            // Assert
            Assert.Equal("Build pipeline contains credential scan task", ((IRule)rule).Description);
            Assert.Equal("https://confluence.dev.somecompany.nl/x/LorHDQ", ((IRule)rule).Link);
        }

        [Theory]
        [InlineData("f0462eae-4df1-45e9-a754-8184da95ed01", "dbe519ee-a2e4-43f5-8e1a-949bd935b736", true)]
        [InlineData("SomethingWrong", "dbe519ee-a2e4-43f5-8e1a-949bd935b736", false)]
        public async Task GivenGuiBuildPipeline_WhenCredScanTask_ThenEvaluatesToExpectedResult(string credScanTaskId,
            string postAnalysisTaskId,
            bool expectedResult)
        {
            //Assert
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 1));
            _fixture.Customize<BuildPhase>(ctx => ctx
                .With(bp => bp.Steps, new List<BuildStep>
                {
                    new BuildStep {Enabled = true, Task = new BuildTask {Id = credScanTaskId}},
                    new BuildStep
                    {
                        Enabled = true, Task = new BuildTask {Id = postAnalysisTaskId},
                        Inputs = new Dictionary<string, string> {{"CredScan", "true"}}
                    }
                }));

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Project>();
            var client = Substitute.For<IVstsRestClient>();

            //Act
            var rule = new BuildPipelineHasCredScanTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            //Assert
            result.ShouldBe(expectedResult);
        }

        [Theory]
        [InlineData("CredScan", "PostAnalysis", false)]
        [InlineData("CredScanOdd", "PostAnalysis", false)]
        public async Task GivenYamlBuildPipeline_WhenCredScanTask_ThenEvaluatesToExpectedResult(string credScanTaskName,
            string postAnalysisTaskName, bool expectedResult)
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));


            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Project>();

            var yamlResponse = new YamlPipelineResponse { FinalYaml = $"steps:\r- task: {credScanTaskName}\r- task: {postAnalysisTaskName}" };
            var client = Substitute.For<IVstsRestClient>();
            client.PostAsync(Arg.Any<IVstsRequest<YamlPipelineRequest, YamlPipelineResponse>>(), Arg.Any<YamlPipelineRequest>()).Returns(yamlResponse);

            var rule = new BuildPipelineHasCredScanTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(expectedResult);
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("123", false)]
        public async Task GivenYamlBuildPipeline_WhenCredScanTaskWithValidInput_ThenEvaluatesToTrue(
            string propertyValue, bool expectedResult)
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Project>();

            var yamlResponse = new YamlPipelineResponse { FinalYaml = $@"
steps:
- task: CredScan
- task: PostAnalysis
  inputs:
      CredScan: {propertyValue}
      ToolLogsNotFoundAction: 'Error'" };

            var client = Substitute.For<IVstsRestClient>();
            client.PostAsync(Arg.Any<IVstsRequest<YamlPipelineRequest, YamlPipelineResponse>>(), Arg.Any<YamlPipelineRequest>()).Returns(yamlResponse);

            var rule = new BuildPipelineHasCredScanTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(expectedResult);
        }
    }
}
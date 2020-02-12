using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Newtonsoft.Json.Linq;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using Project = SecurePipelineScan.VstsService.Requests.Project;
using Repository = SecurePipelineScan.VstsService.Response.Repository;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class BuildPipelineHasCredScanTaskTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly Fixture _fixture = new Fixture {RepeatCount = 1};

        public BuildPipelineHasCredScanTaskTests(TestConfig config)
        {
            _config = config;
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateIntegrationTest_gui()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var project = await client.GetAsync(Project.ProjectById(_config.Project));
            var buildPipeline = await client.GetAsync(Builds.BuildDefinition(project.Id, "2"))
                .ConfigureAwait(false); // 'SOx-compliant-demo-ASP.NET Core-CI' pipeline

            var rule = new BuildPipelineHasCredScanTask(client);
            (await rule.EvaluateAsync(project, buildPipeline)).GetValueOrDefault().ShouldBeTrue();
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateIntegrationTest_yaml()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var project = await client.GetAsync(Project.ProjectById(_config.Project));
            var buildPipeline = await client.GetAsync(Builds.BuildDefinition(project.Id, "275"))
                .ConfigureAwait(false); // 'NestedYamlTemplates' pipeline

            var rule = new BuildPipelineHasCredScanTask(client);
            (await rule.EvaluateAsync(project, buildPipeline)).GetValueOrDefault().ShouldBeTrue();
        }

        [Fact]
        public void BuildPipelineHasCredScanTask_ShouldHaveCorrectProperties()
        {
            // Arrange
            var client = Substitute.For<IVstsRestClient>();
            var rule = new BuildPipelineHasCredScanTask(client);

            // Assert
            Assert.Equal("Build pipeline contains credential scan task", ((IRule) rule).Description);
            Assert.Equal("https://confluence.dev.somecompany.nl/x/LorHDQ", ((IRule) rule).Link);
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
                    new BuildStep {Enabled = true, Task = new BuildTask {Id = postAnalysisTaskId}}
                }));

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<VstsService.Response.Project>();
            var client = Substitute.For<IVstsRestClient>();

            //Act
            var rule = new BuildPipelineHasCredScanTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            //Assert
            result.ShouldBe(expectedResult);
        }

        [Theory]
        [InlineData("CredScan", "PostAnalysis", true)]
        [InlineData("CredScanOdd", "PostAnalysis", false)]
        public async Task GivenYamlBuildPipeline_WhenCredScanTask_ThenEvaluatesToExpectedResult(string credScanTaskName,
            string postAnalysisTaskName, bool expectedResult)
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<VstsService.Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var gitItem = new JObject
            {
                {"content", $"steps:\r- task: {credScanTaskName}\r- task: {postAnalysisTaskName}"}
            };

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<VstsService.Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new BuildPipelineHasCredScanTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(expectedResult);
        }
    }
}
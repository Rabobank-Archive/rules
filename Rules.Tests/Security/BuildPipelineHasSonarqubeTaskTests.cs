using System;
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
    public class BuildPipelineHasSonarqubeTaskTests
    {
        private readonly Fixture _fixture = new Fixture { RepeatCount = 1 };

        public BuildPipelineHasSonarqubeTaskTests()
        {
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        public async Task GivenPipeline_WhenYamlFileWithSonarTask_ThenEvaluatesToTrue()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Project>();

            var yamlResponse = new YamlPipelineResponse { FinalYaml = "steps:\r- task: SonarQubeAnalyze@3" };
            var client = Substitute.For<IVstsRestClient>();
            client.PostAsync(Arg.Any<IVstsRequest<YamlPipelineRequest, YamlPipelineResponse>>(), Arg.Any<YamlPipelineRequest>()).Returns(yamlResponse);

            var rule = new BuildPipelineHasSonarqubeTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenYamlFileWithoutSonarTask_ThenEvaluatesToFalse()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Project>();

            var yamlResponse = new YamlPipelineResponse { FinalYaml = "steps:\r- task: OtherTask" };
            var client = Substitute.For<IVstsRestClient>();
            client.PostAsync(Arg.Any<IVstsRequest<YamlPipelineRequest, YamlPipelineResponse>>(), Arg.Any<YamlPipelineRequest>()).Returns(yamlResponse);

            var rule = new BuildPipelineHasSonarqubeTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(false);
        }
    }
}
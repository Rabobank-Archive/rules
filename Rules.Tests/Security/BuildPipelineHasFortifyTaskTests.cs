using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
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
    public class BuildPipelineHasFortifyTaskTests
    {
        private readonly Fixture _fixture = new Fixture { RepeatCount = 1 };
        private const string TaskName = "FortifySCA";

        public BuildPipelineHasFortifyTaskTests()
        {
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        #region [ gui ]

        #region [ nested taskgroup ]

        [Fact]
        public async Task GivenPipeline_WhenNestedTaskGroupWithFortifyTask_ThenEvaluatesToTrue()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 1));
            _fixture.Customize<Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));
            _fixture.Customize<BuildStep>(ctx => ctx
                .With(b => b.Enabled, true));
            _fixture.Customize<BuildTask>(ctx => ctx
                .With(t => t.DefinitionType, "metaTask")
                .With(t => t.Id, "df6aa8e5-82dc-468c-a794-a7990523363d"));

            var fortifyStep = new BuildStep
            {
                Enabled = true,
                Task = new BuildTask
                {
                    Id = "818386e5-c8a5-46c3-822d-954b3c8fb130"
                }
            };

            var taskGroup = new TaskGroup { Tasks = new[] { fortifyStep } };
            var taskGroupResponse = new TaskGroupResponse { Value = new List<TaskGroup> { taskGroup } };

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<TaskGroupResponse>>()).Returns(taskGroupResponse);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false);

            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenNestedTaskGroupWithCircularDependencyAndNoFortifyTask_ThenEvaluatesToFalse()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 1));
            _fixture.Customize<Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));
            _fixture.Customize<BuildStep>(ctx => ctx
                .With(b => b.Enabled, true));
            _fixture.Customize<BuildTask>(ctx => ctx
                .With(t => t.DefinitionType, "metaTask")
                .With(t => t.Id, "df6aa8e5-82dc-468c-a794-a7990523363d"));

            var circularStep = new BuildStep
            {
                Enabled = true,
                Task = new BuildTask
                {
                    Id = "df6aa8e5-82dc-468c-a794-a7990523363d",
                    DefinitionType = "metaTask"
                }
            };

            var taskGroup = new TaskGroup { Tasks = new[] { circularStep } };
            var taskGroupResponse = new TaskGroupResponse { Value = new List<TaskGroup> { taskGroup } };

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<TaskGroupResponse>>()).Returns(taskGroupResponse);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false);

            result.ShouldBe(false);
        }

        #endregion

        #endregion

        #region [ yaml ]

        [Fact]
        public async Task GivenPipeline_WhenStepsYamlFileWithFortifyTask_ThenEvaluatesToTrue()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Project>();

            var yamlResponse = new YamlPipelineResponse { FinalYaml = $"steps:\r- task: {TaskName}@5" };
            var client = Substitute.For<IVstsRestClient>();
            client.PostAsync(Arg.Any<IVstsRequest<YamlPipelineRequest, YamlPipelineResponse>>(), Arg.Any<YamlPipelineRequest>()).Returns(yamlResponse);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false);

            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenJobsYamlFileWithFortifyTask_ThenEvaluatesToTrue()
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
jobs:
- job: JobName
  steps:
  - task: {TaskName}@5" };
            var client = Substitute.For<IVstsRestClient>();
            client.PostAsync(Arg.Any<IVstsRequest<YamlPipelineRequest, YamlPipelineResponse>>(), Arg.Any<YamlPipelineRequest>()).Returns(yamlResponse);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false);

            result.ShouldBe(true);
        }

        [Theory]
        [InlineData("Fortify_SCA@5")]
        [InlineData("_FortifySCA@5")]
        [InlineData("FortifySCA_@5")]
        [InlineData("Fortify_SCA")]
        [InlineData("_FortifySCA")]
        [InlineData("FortifySCA_")]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public async Task GivenPipeline_WhenYamlFileWithoutFortifyTask_ThenEvaluatesToFalse(string fortifyTask)
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Project>();

            var yamlResponse = new YamlPipelineResponse { FinalYaml = $"steps:\r- task: {fortifyTask}" };
            var client = Substitute.For<IVstsRestClient>();
            client.PostAsync(Arg.Any<IVstsRequest<YamlPipelineRequest, YamlPipelineResponse>>(), Arg.Any<YamlPipelineRequest>()).Returns(yamlResponse);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline).ConfigureAwait(false);

            result.ShouldBe(false);
        }

        #endregion
    }
}
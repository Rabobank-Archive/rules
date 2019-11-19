using AutoFixture;
using AutoFixture.AutoNSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using System;
using Newtonsoft.Json.Linq;
using NSubstitute;
using SecurePipelineScan.VstsService.Response;
using System.Collections.Generic;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class BuildPipelineHasFortifyTaskTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly Fixture _fixture = new Fixture { RepeatCount = 1 };

        public BuildPipelineHasFortifyTaskTests(TestConfig config)
        {
            _config = config;
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        [Trait("category", "integration")]
        public async System.Threading.Tasks.Task EvaluateIntegrationTest_gui()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var project = await client.GetAsync(VstsService.Requests.Project.ProjectById(_config.Project));
            var buildPipeline = await client.GetAsync(Builds.BuildDefinition(project.Id, "2"))
                .ConfigureAwait(false);

            var rule = new BuildPipelineHasFortifyTask(client);
            (await rule.EvaluateAsync(project, buildPipeline)).GetValueOrDefault().ShouldBeTrue();
        }

        [Fact]
        [Trait("category", "integration")]
        public async System.Threading.Tasks.Task EvaluateIntegrationTest_yaml()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var project = await client.GetAsync(VstsService.Requests.Project.ProjectById(_config.Project));
            var buildPipeline = await client.GetAsync(Builds.BuildDefinition(project.Id, "197"))
                .ConfigureAwait(false);

            var rule = new BuildPipelineHasFortifyTask(client);
            (await rule.EvaluateAsync(project, buildPipeline)).GetValueOrDefault().ShouldBeFalse();
        }

        [Fact]
        public async System.Threading.Tasks.Task GivenPipeline_WhenYamlFileWithFortifyTask_ThenEvaluatesToTrue()
        {
            _fixture.Customize<Response.BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var gitItem = new JObject
            {
                { "content", "steps:\r- task: FortifySCA@5" }
            };

            var buildPipeline = _fixture.Create<Response.BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(true);
        }

        [Fact]
        public async System.Threading.Tasks.Task GivenPipeline_WhenYamlFileWithoutFortifyTask_ThenEvaluatesToFalse()
        {
            _fixture.Customize<Response.BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var gitItem = new JObject
            {
                { "content", "steps:\r- task: OtherTask" }
            };

            var buildPipeline = _fixture.Create<Response.BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(false);
        }

        [Fact]
        public async System.Threading.Tasks.Task GivenPipeline_WhenNestedTaskGroupWithFortifyTask_ThenEvaluatesToTrue()
        {
            _fixture.Customize<Response.BuildProcess>(ctx => ctx
                .With(p => p.Type, 1));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));
            _fixture.Customize<Response.BuildStep>(ctx => ctx
                .With(b => b.Enabled, true));
            _fixture.Customize<Response.BuildTask>(ctx => ctx
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
            var taskGroup = new Response.TaskGroup();
            taskGroup.Tasks = new BuildStep[] { fortifyStep };

            var taskGroupResponse = new TaskGroupResponse();
            taskGroupResponse.Value = new List<Response.TaskGroup>() { taskGroup };

            var buildPipeline = _fixture.Create<Response.BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<TaskGroupResponse>>()).Returns(taskGroupResponse);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(true);
        }

        [Fact]
        public async System.Threading.Tasks.Task GivenPipeline_WhenNestedTaskGroupWithCircularDependencyAndNoFortifyTask_ThenEvaluatesToFalse()
        {
            _fixture.Customize<Response.BuildProcess>(ctx => ctx
                .With(p => p.Type, 1));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));
            _fixture.Customize<Response.BuildStep>(ctx => ctx
                .With(b => b.Enabled, true));
            _fixture.Customize<Response.BuildTask>(ctx => ctx
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
            var taskGroup = new Response.TaskGroup();
            taskGroup.Tasks = new BuildStep[] { circularStep };

            var taskGroupResponse = new TaskGroupResponse();
            taskGroupResponse.Value = new List<Response.TaskGroup>() { taskGroup };

            var buildPipeline = _fixture.Create<Response.BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<TaskGroupResponse>>()).Returns(taskGroupResponse);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(false);
        }
    }
}
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
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class BuildPipelineHasFortifyTaskTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly Fixture _fixture = new Fixture { RepeatCount = 1 };
        private const string TaskName = "FortifySCA";

        public BuildPipelineHasFortifyTaskTests(TestConfig config)
        {
            _config = config;
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateIntegrationTest_gui()
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
        public async Task EvaluateIntegrationTest_yaml()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var project = await client.GetAsync(VstsService.Requests.Project.ProjectById(_config.Project));
            var buildPipeline = await client.GetAsync(Builds.BuildDefinition(project.Id, "197"))
                .ConfigureAwait(false);

            var rule = new BuildPipelineHasFortifyTask(client);
            (await rule.EvaluateAsync(project, buildPipeline)).GetValueOrDefault().ShouldBeFalse();
        }

        [Fact(Skip = "Nested YAML Pipeline should first be fixed")]
        [Trait("category", "integration")]
        public async Task EvaluateNestedTemplatesIntegrationTest_yaml()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var project = await client.GetAsync(VstsService.Requests.Project.ProjectById(_config.Project));
            var buildPipeline = await client.GetAsync(Builds.BuildDefinition(project.Id, "275"))
                .ConfigureAwait(false);

            var rule = new BuildPipelineHasFortifyTask(client);
            (await rule.EvaluateAsync(project, buildPipeline)).GetValueOrDefault().ShouldBeTrue();
        }

        [Fact]
        public async Task GivenPipeline_WhenYamlFileWithFortifyTask_ThenEvaluatesToTrue()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var gitItem = new JObject
            {
                { "content", $"steps:\r- task: {TaskName}@5" }
            };

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenNestedYamlTemplateWithFortifyTask_ThenEvaluatesToTrue()
        {
            // arrange
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2)
                .With(p => p.YamlFilename, "azure-pipelines.yml"));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var azurePipelineGitItem = new JObject
            {
                { "content", "steps:\r- template: steps-template.yml" }
            };

            var stepsTemplateGitItem = new JObject
            {
                { "content", $"steps:\r- task: {TaskName}@5" }
            };

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                r => r.QueryParams["path"].ToString() == "azure-pipelines.yml"))
                .Returns(azurePipelineGitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "steps-template.yml"))
                .Returns(stepsTemplateGitItem);

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var rule = new BuildPipelineHasFortifyTask(client);

            // act
            var result = await rule.EvaluateAsync(project, buildPipeline);

            // assert
            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenNestedYamlTemplateWithFortifyTaskNotFound_ThenEvaluatesToNull()
        {
            // arrange
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2)
                .With(p => p.YamlFilename, "azure-pipelines.yml"));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var azurePipelineGitItem = new JObject
            {
                { "content", "steps:\r- template: steps-template.yml" }
            };

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "azure-pipelines.yml"))
                .Returns(azurePipelineGitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "steps-template.yml"))
                .Returns((JObject)null);

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var rule = new BuildPipelineHasFortifyTask(client);

            // act
            var result = await rule.EvaluateAsync(project, buildPipeline);

            // assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GivenPipeline_WhenMultipleNestedYamlTemplatesWithFortifyTask_ThenEvaluatesToTrue()
        {
            // arrange
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2)
                .With(p => p.YamlFilename, "azure-pipelines.yml"));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var azurePipelineGitItem = new JObject
            {
                {"content", "steps:\r- template: steps-template1.yml"}
            };

            var stepsTemplate1GitItem = new JObject
            {
                {"content", "steps:\r- template: steps-template2.yml"}
            };

            var stepsTemplate2GitItem = new JObject
            {
                {"content", "steps:\r- template: steps-template3.yml"}
            };

            var stepsTemplate3GitItem = new JObject
            {
                {"content", $"steps:\r- task: {TaskName}@5"}
            };

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "azure-pipelines.yml"))
                .Returns(azurePipelineGitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "steps-template1.yml"))
                .Returns(stepsTemplate1GitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "steps-template2.yml"))
                .Returns(stepsTemplate2GitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "steps-template3.yml"))
                .Returns(stepsTemplate3GitItem);

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var rule = new BuildPipelineHasFortifyTask(client);

            // act
            var result = await rule.EvaluateAsync(project, buildPipeline);

            // assert
            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenTooManyNestedYamlTemplateLevelsWithFortifyTask_ThenEvaluatesToNull()
        {
            // arrange
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2)
                .With(p => p.YamlFilename, "azure-pipelines.yml"));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var azurePipelineGitItem = new JObject
            {
                {"content", "steps:\r- template: steps-template1.yml"}
            };

            var stepsTemplate1GitItem = new JObject
            {
                {"content", "steps:\r- template: steps-template2.yml"}
            };

            var stepsTemplate2GitItem = new JObject
            {
                {"content", "steps:\r- template: steps-template3.yml"}
            };

            var stepsTemplate3GitItem = new JObject
            {
                {"content", "steps:\r- template: steps-template4.yml"}
            };

            var stepsTemplate4GitItem = new JObject
            {
                {"content", $"steps:\r- task: {TaskName}@5"}
            };

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "azure-pipelines.yml"))
                .Returns(azurePipelineGitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "steps-template1.yml"))
                .Returns(stepsTemplate1GitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "steps-template2.yml"))
                .Returns(stepsTemplate2GitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "steps-template3.yml"))
                .Returns(stepsTemplate3GitItem);
            client.GetAsync(Arg.Is<IVstsRequest<JObject>>(
                    r => r.QueryParams["path"].ToString() == "steps-template4.yml"))
                .Returns(stepsTemplate4GitItem);

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var rule = new BuildPipelineHasFortifyTask(client);

            // act
            var result = await rule.EvaluateAsync(project, buildPipeline);

            // assert
            result.ShouldBeNull();
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
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var gitItem = new JObject
            {
                { "content", $"steps:\r- task: {fortifyTask}" }
            };

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(false);
        }

        [Fact]
        public async Task GivenPipeline_WhenNestedTaskGroupWithFortifyTask_ThenEvaluatesToTrue()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 1));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
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

            var taskGroup = new Response.TaskGroup {Tasks = new[] {fortifyStep}};
            var taskGroupResponse = new TaskGroupResponse {Value = new List<Response.TaskGroup> {taskGroup}};

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<TaskGroupResponse>>()).Returns(taskGroupResponse);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenNestedTaskGroupWithCircularDependencyAndNoFortifyTask_ThenEvaluatesToFalse()
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 1));
            _fixture.Customize<Response.Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Response.Repository>(ctx => ctx
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

            var taskGroup = new Response.TaskGroup {Tasks = new[] {circularStep}};
            var taskGroupResponse = new TaskGroupResponse {Value = new List<Response.TaskGroup>() {taskGroup}};

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Response.Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<TaskGroupResponse>>()).Returns(taskGroupResponse);

            var rule = new BuildPipelineHasFortifyTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(false);
        }
    }
}
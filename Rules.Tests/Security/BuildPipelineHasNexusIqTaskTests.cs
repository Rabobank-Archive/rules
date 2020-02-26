using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
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
    public class BuildPipelineHasNexusIqTaskTests
    {
        private readonly Fixture _fixture = new Fixture {RepeatCount = 1};

        public BuildPipelineHasNexusIqTaskTests()
        {
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        [Theory]
        [InlineData("4f40d1a2-83b0-4ddc-9a77-e7f279eb1802", true)]
        [InlineData("_4f40d1a2-83b0-4ddc-9a77-e7f279eb1802", false)]
        [InlineData("4f40d1a2-83b0-4ddc-9a77-e7f279eb1802_", false)]
        [InlineData(" 4f40d1a2-83b0-4ddc-9a77-e7f279eb1802", false)]
        [InlineData("4f40d1a2-83b0-4ddc-9a77-e7f279eb1802 ", false)]
        [InlineData("SomeThingWeird", false)]
        [InlineData(" ", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public async Task GivenGuiBuildPipeline_WhenNexusIqTask_ThenEvaluatesToExpectedResult(string taskId,
            bool expectedResult)
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 1));
            _fixture.Customize<BuildStep>(ctx => ctx
                .With(s => s.Enabled, true));
            _fixture.Customize<BuildTask>(ctx => ctx
                .With(t => t.Id, taskId));

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Project>();
            var client = Substitute.For<IVstsRestClient>();

            //Act
            var rule = new BuildPipelineHasNexusIqTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            //Assert
            result.ShouldBe(expectedResult);
        }

        [Theory]
        [InlineData("SonatypeIntegrations.nexus-iq-azure-extension.nexus-iq-azure-pipeline-task.NexusIqPipelineTask@1",
            true)]
        [InlineData("SonatypeIntegrations.nexus-iq-azure-extension.nexus-iq-azure-pipeline-task.NexusIqPipelineTask@2",
            true)]
        [InlineData("SonatypeIntegrations.nexus-iq-azure-extension.nexus-iq-azure-pipeline-task.NexusIqPipelineTask",
            true)]
        [InlineData("SonatypeIntegrations.nexus-iq-azure-extension.nexus-iq-azure-pipeline-task.NexusIqPipelineTask_@1",
            false)]
        [InlineData("SonatypeIntegrations.nexus-iq-azure-extension.nexus-iq-azure-pipeline-task._NexusIqPipelineTask@2",
            false)]
        [InlineData("SonatypeIntegrations.nexus-iq-azure-extension.nexus-iq-azure-pipeline-task.NexusIqPipelineTask_",
            false)]
        [InlineData("SonatypeIntegrations.nexus-iq-azure-extension.nexus-iq-azure-pipeline-task._NexusIqPipelineTask",
            false)]
        [InlineData("SomeThingWeird", false)]
        [InlineData(" ", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public async Task GivenYamlBuildPipeline_WhenNexusIqTask_ThenEvaluatesToExpectedResult(string taskName,
            bool expectedResult)
        {
            _fixture.Customize<BuildProcess>(ctx => ctx
                .With(p => p.Type, 2));
            _fixture.Customize<Project>(ctx => ctx
                .With(x => x.Name, "projectA"));
            _fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Url, new Uri("https://projectA.nl")));

            var gitItem = new JObject
            {
                {"content", $"steps:\r- task: {taskName}"}
            };

            var buildPipeline = _fixture.Create<BuildDefinition>();
            var project = _fixture.Create<Project>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<JObject>>()).Returns(gitItem);

            var rule = new BuildPipelineHasNexusIqTask(client);
            var result = await rule.EvaluateAsync(project, buildPipeline);

            result.ShouldBe(expectedResult);
        }
    }
}
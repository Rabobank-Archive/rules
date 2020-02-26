using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class ReleasePipelineHasSm9ChangeTaskTests : IClassFixture<TestConfig>
    {
        private readonly Fixture _fixture = new Fixture {RepeatCount = 1};

        public ReleasePipelineHasSm9ChangeTaskTests()
        {
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        public async Task GivenPipeline_WhenEnabledSM9Task_ThenEvaluatesToTrue()
        {
            //Arrange
            _fixture.Customize<WorkflowTask>(ctx => ctx
                .With(t => t.TaskId, new Guid("d0c045b6-d01d-4d69-882a-c21b18a35472"))
                .With(t => t.Enabled, true));
            var projectId = _fixture.Create<string>();
            var releasePipeline = _fixture.Create<ReleaseDefinition>();
            var client = Substitute.For<IVstsRestClient>();

            //Act
            var rule = new ReleasePipelineHasSm9ChangeTask(client);
            var result = await rule.EvaluateAsync(projectId, releasePipeline);

            //Assert
            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenDisabledSM9Task_ThenEvaluatesToFalse()
        {
            //Arrange
            _fixture.Customize<WorkflowTask>(ctx => ctx
                .With(t => t.TaskId, new Guid("d0c045b6-d01d-4d69-882a-c21b18a35472"))
                .With(t => t.Enabled, false));
            var projectId = _fixture.Create<string>();
            var releasePipeline = _fixture.Create<ReleaseDefinition>();
            var client = Substitute.For<IVstsRestClient>();

            //Act
            var rule = new ReleasePipelineHasSm9ChangeTask(client);
            var result = await rule.EvaluateAsync(projectId, releasePipeline);

            //Assert
            result.ShouldBe(false);
        }

        [Fact]
        public async Task GivenPipeline_WhenNoSM9Task_ThenEvaluatesToFalse()
        {
            //Arrange
            var projectId = _fixture.Create<string>();
            var releasePipeline = _fixture.Create<ReleaseDefinition>();
            var client = Substitute.For<IVstsRestClient>();

            //Act
            var rule = new ReleasePipelineHasSm9ChangeTask(client);
            var result = await rule.EvaluateAsync(projectId, releasePipeline);

            //Assert
            result.ShouldBe(false);
        }

        [Fact]
        public async Task GivenPipeline_WhenEnabledTaskGroupWithSM9Task_ThenEvaluatesToTrue()
        {
            //Arrange
            _fixture.Customize<WorkflowTask>(ctx => ctx
                .With(t => t.DefinitionType, "metaTask")
                .With(t => t.Enabled, true));
            _fixture.Customize<BuildStep>(ctx => ctx
                .With(s => s.Enabled, true));
            _fixture.Customize<BuildTask>(ctx => ctx
                .With(t => t.Id, "d0c045b6-d01d-4d69-882a-c21b18a35472"));

            var taskGroupResponse = _fixture.Create<TaskGroupResponse>();
            var projectId = _fixture.Create<string>();
            var releasePipeline = _fixture.Create<ReleaseDefinition>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<TaskGroupResponse>>()).Returns(taskGroupResponse);

            //Act
            var rule = new ReleasePipelineHasSm9ChangeTask(client);
            var result = await rule.EvaluateAsync(projectId, releasePipeline);

            //Assert
            result.ShouldBe(true);
        }

        [Fact]
        public async Task GivenPipeline_WhenDisabledTaskGroupWithSM9Task_ThenEvaluatesToFalse()
        {
            //Arrange
            _fixture.Customize<WorkflowTask>(ctx => ctx
                .With(t => t.DefinitionType, "metaTask")
                .With(t => t.Enabled, false));
            _fixture.Customize<BuildStep>(ctx => ctx
                .With(s => s.Enabled, true));
            _fixture.Customize<BuildTask>(ctx => ctx
                .With(t => t.Id, "d0c045b6-d01d-4d69-882a-c21b18a35472"));

            var taskGroupResponse = _fixture.Create<TaskGroupResponse>();
            var projectId = _fixture.Create<string>();
            var releasePipeline = _fixture.Create<ReleaseDefinition>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<TaskGroupResponse>>()).Returns(taskGroupResponse);

            //Act
            var rule = new ReleasePipelineHasSm9ChangeTask(client);
            var result = await rule.EvaluateAsync(projectId, releasePipeline);

            //Assert
            result.ShouldBe(false);
        }

        [Fact]
        public async Task GivenPipeline_WhenTaskGroupWithoutSM9Task_ThenEvaluatesToFalse()
        {
            //Arrange
            _fixture.Customize<WorkflowTask>(ctx => ctx
                .With(t => t.DefinitionType, "metaTask")
                .With(t => t.Enabled, true));
            _fixture.Customize<BuildStep>(ctx => ctx
                .With(s => s.Enabled, true));

            var taskGroupResponse = _fixture.Create<TaskGroupResponse>();
            var projectId = _fixture.Create<string>();
            var releasePipeline = _fixture.Create<ReleaseDefinition>();

            var client = Substitute.For<IVstsRestClient>();
            client.GetAsync(Arg.Any<IVstsRequest<TaskGroupResponse>>()).Returns(taskGroupResponse);

            //Act
            var rule = new ReleasePipelineHasSm9ChangeTask(client);
            var result = await rule.EvaluateAsync(projectId, releasePipeline);

            //Assert
            result.ShouldBe(false);
        }
    }
}
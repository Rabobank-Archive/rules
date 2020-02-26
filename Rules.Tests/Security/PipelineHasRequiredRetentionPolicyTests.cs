using System.Linq;
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
    public class PipelineHasRequiredRetentionPolicyTests
    {
        private const string PipelineId = "1";
        private readonly IFixture _fixture = new Fixture {RepeatCount = 1}.Customize(new AutoNSubstituteCustomization());
        private readonly IVstsRestClient _client = Substitute.For<IVstsRestClient>();
        
        [Fact]
        public async Task EvaluateShouldReturnTrueWhenPipelineHasRequiredRetentionPolicy()
        {
            //Arrange
            // ReSharper disable twice RedundantArgumentDefaultValue
            CustomizePolicySettings(_fixture, 450, true);
            SetupClient(_client, _fixture);
            var releasePipeline = _fixture.Create<ReleaseDefinition>();

            //Act
            var rule = new PipelineHasRequiredRetentionPolicy(_client);
            var result = await rule.EvaluateAsync("", releasePipeline);

            //Assert
            result.ShouldBe(true);
        }

        [Fact]
        public async Task EvaluateShouldReturnFalseWhenAnyStageWithinPipelineDoesNotHaveRequiredRetentionPolicy()
        {
            //Arrange
            // ReSharper disable twice RedundantArgumentDefaultValue
            CustomizePolicySettings(_fixture, 450, true);
            SetupClient(_client, _fixture);
            var releasePipeline = _fixture.Create<ReleaseDefinition>();

            if (releasePipeline.Environments.Any())
            {
                releasePipeline.Environments.First().RetentionPolicy.DaysToKeep = 0;
            }

            //Act
            var rule = new PipelineHasRequiredRetentionPolicy(_client);
            var result = await rule.EvaluateAsync("", releasePipeline);

            //Assert 
            Assert.False(result);
        }

        [Fact]
        public async Task EvaluateShouldReturnFalseWhenReleasesAreRetainedShorterThenRequired()
        {
            //Arrange
            // ReSharper disable once RedundantArgumentDefaultValue
            CustomizePolicySettings(_fixture, 5, true);
            SetupClient(_client, _fixture);
            var releasePipeline = _fixture.Create<ReleaseDefinition>();

            //Act
            var rule = new PipelineHasRequiredRetentionPolicy(_client);
            var result = await rule.EvaluateAsync("", releasePipeline);

            //Assert
            result.ShouldBe(false);
        }

        [Fact]
        public async Task EvaluateShouldReturnFalseWhenRetainBuildsIsDisabled()
        {
            //Arrange
            CustomizePolicySettings(_fixture, 500, false);
            SetupClient(_client, _fixture);
            var releasePipeline = _fixture.Create<ReleaseDefinition>();

            //Act
            var rule = new PipelineHasRequiredRetentionPolicy(_client);
            var result = await rule.EvaluateAsync("", releasePipeline);

            //Assert
            result.ShouldBe(false);
        }

        [Fact]
        public async Task GivenPolicySettingsAreNotCorrect_WhenReconcile_ThenSettingsArePut()
        {
            //Arrange
            CustomizePolicySettings(_fixture, 10, false);
            SetupClient(_client, _fixture);

            //Act
            var rule = new PipelineHasRequiredRetentionPolicy(_client) as IReconcile;
            await rule.ReconcileAsync("", PipelineId);

            // Assert
            await _client
                .Received()
                .PutAsync(Arg.Any<IVstsRequest<ReleaseSettings>>(), Arg.Any<ReleaseSettings>());
        }

        [Fact]
        public async Task GivenPolicySettingsAreCorrect_WhenReconcile_ThenPipelineIsUpdatedAnyway()
        {
            //Arrange
            CustomizePolicySettings(_fixture);
            SetupClient(_client, _fixture);

            //Act
            var rule = new PipelineHasRequiredRetentionPolicy(_client) as IReconcile;
            await rule.ReconcileAsync("", PipelineId);

            // Assert
            await _client
                .Received()
                .PutAsync(Arg.Any<IVstsRequest<object>>(), Arg.Any<JObject>());
        }

        private static void CustomizePolicySettings(IFixture fixture, int daysToKeep = 450,
            bool retainBuild = true)
        {
            fixture.Customize<RetentionPolicy>(ctx => ctx
                .With(r => r.DaysToKeep, daysToKeep)
                .With(r => r.RetainBuild, retainBuild));
        }

        private static void SetupClient(IVstsRestClient client, IFixture fixture)
        {
            client
                .GetAsync(Arg.Any<IVstsRequest<ReleaseDefinition>>())
                .Returns(fixture.Create<ReleaseDefinition>());

            client
                .GetAsync(Arg.Any<IVstsRequest<ReleaseSettings>>())
                .Returns(fixture.Create<ReleaseSettings>());


            client
                .GetAsync(Arg.Any<IVstsRequest<JObject>>())
                .Returns(new JObject());
        }
    }
}
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Newtonsoft.Json.Linq;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using System.Linq;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class PipelineHasRequiredRetentionPolicyTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private const string PipelineId = "1";
        private readonly Fixture _fixture = new Fixture { RepeatCount = 1 };
        private readonly IVstsRestClient _client = Substitute.For<IVstsRestClient>();

        public PipelineHasRequiredRetentionPolicyTests(TestConfig config)
        {
            _config = config;
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        public async Task EvaluateIntegrationTest()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, PipelineId))
                .ConfigureAwait(false); 
            
            //Act
            var rule = new PipelineHasRequiredRetentionPolicy(client);
            var result = await rule.EvaluateAsync(_config.Project, _config.stageId, releasePipeline);

            //Assert
            result.ShouldBe(true);
        }

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
            var result = await rule.EvaluateAsync(_config.Project, _config.stageId, releasePipeline);

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
                releasePipeline.Environments.First().RetentionPolicy.DaysToKeep = 0;

            //Act
            var rule = new PipelineHasRequiredRetentionPolicy(_client);
            var result = await rule.EvaluateAsync(_config.Project, _config.stageId, releasePipeline);

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
            var result = await rule.EvaluateAsync(_config.Project, _config.stageId, releasePipeline);

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
            var result = await rule.EvaluateAsync(_config.Project, _config.stageId, releasePipeline);

            //Assert
            result.ShouldBe(false) ;
        }

        [Fact]
        public async Task Reconcile()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token);

            //Act
            var rule = new PipelineHasRequiredRetentionPolicy(client) as IReconcile; 
            await rule.ReconcileAsync(_config.Project, null, PipelineId);
        }

        [Fact]
        public async Task GivenPolicySettingsAreNotCorrect_WhenReconcile_ThenSettingsArePut()
        {
            //Arrange
            CustomizePolicySettings(_fixture, 10, false);
            SetupClient(_client, _fixture);

            //Act
            var rule = new PipelineHasRequiredRetentionPolicy(_client) as IReconcile; 
            await rule.ReconcileAsync(_config.Project, null, PipelineId);

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
            await rule.ReconcileAsync(_config.Project, null, PipelineId);

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
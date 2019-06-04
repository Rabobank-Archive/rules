using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class PipelineHasRequiredRetentionPolicyTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IRestClientFactory _factory;
        private const string PipelineId = "1";
        private readonly Fixture _fixture = new Fixture { RepeatCount = 1 };
        private readonly IVstsRestClient _client = Substitute.For<IVstsRestClient>();

        public PipelineHasRequiredRetentionPolicyTests(TestConfig config)
        {
            _config = config;
            _factory = new RestClientFactory();
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        public void EvaluateIntegrationTest()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token, _factory);

            //Act
            var rule = new PipelineHasRequiredRetentionPolicy(client);
            rule.Evaluate(_config.Project, PipelineId);
        }

        [Fact]
        public void EvaluateShouldReturnTrueWhenPipelineHasRequiredRetentionPolicy()
        {
            //Arrange
            // ReSharper disable twice RedundantArgumentDefaultValue
            CustomizePolicySettings(_fixture, 450, true);
            SetupClient(_client, _fixture);

            //Act
            var rule = new PipelineHasRequiredRetentionPolicy(_client);
            var evaluatedRule = rule.Evaluate(_config.Project, PipelineId);

            //Assert
            evaluatedRule.ShouldBeTrue();
        }

        [Fact]
        public void EvaluateShouldReturnFalseWhenReleasesAreRetainedShorterThenRequired()
        {
            //Arrange
            // ReSharper disable once RedundantArgumentDefaultValue
            CustomizePolicySettings(_fixture, 5, true);
            SetupClient(_client, _fixture);

            //Act
            var rule = new PipelineHasRequiredRetentionPolicy(_client);
            var evaluatedRule = rule.Evaluate(_config.Project, PipelineId);

            //Assert
            evaluatedRule.ShouldBeFalse();
        }

        [Fact]
        public void EvaluateShouldReturnFalseWhenRetainBuildsIsDisabled()
        {
            //Arrange
            CustomizePolicySettings(_fixture, 500, false);
            SetupClient(_client, _fixture);

            //Act
            var rule = new PipelineHasRequiredRetentionPolicy(_client);
            var evaluatedRule = rule.Evaluate(_config.Project, PipelineId);

            //Assert
            evaluatedRule.ShouldBeFalse();
        }

        [Fact]
        public void Reconcile()
        {
            //Arrange
            var client = new VstsRestClient(_config.Organization, _config.Token, _factory);

            //Act
            var rule = new PipelineHasRequiredRetentionPolicy(client) as IReconcile; 
            rule.Reconcile(_config.Project, PipelineId);
        }

        private static void CustomizePolicySettings(IFixture fixture, int daysToKeep = 450,
            bool retainBuild = true)
        {
            fixture.Customize<RetentionPolicy>(ctx => ctx
                .With(r => r.DaysToKeep, daysToKeep)
                .With(r => r.RetainBuild, retainBuild));
        }

        private static void SetupClient(IVstsRestClient client, IFixture fixture) =>
            client
                .Get(Arg.Any<IVstsRequest<ReleaseDefinition>>())
                .Returns(fixture.Create<ReleaseDefinition>());
    }
}
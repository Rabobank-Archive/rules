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
    public class ReleaseIsRelatedToChangeRequestTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private const string ReleaseId = "6";
        private readonly IVstsRestClient _client = Substitute.For<IVstsRestClient>();

        public ReleaseIsRelatedToChangeRequestTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        public void EvaluateIntegrationTest()
        {
            // Act
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            // Arrange
            var rule = new ReleaseIsRelatedToChangeRequest(client);
            rule.Evaluate(projectId, ReleaseId).ShouldBeTrue();
        }

        [Fact]
        public void EvaluateShouldReturnTrueForReleaseHasSm9ChangeId()
        {
            //Arrange
            _client.Get(Arg.Any<IVstsRestRequest<Release>>())
                .Returns(new Release() {Id = ReleaseId, Tags = new []{ "SM9ChangeId 12345", "Random tag"}});
            
            //Act
            var rule = new ReleaseIsRelatedToChangeRequest(_client);
            var evaluatedRule = rule.Evaluate(_config.Project, ReleaseId);

            //Assert
            evaluatedRule.ShouldBeTrue();
        }

        [Fact]
        public void EvaluateShouldReturnFalseForReleaseHasNoSm9ChangeId()
        {
            //Arrange
            _client.Get(Arg.Any<IVstsRestRequest<Release>>())
                .Returns(new Release() {Id = ReleaseId, Tags = new []{ "12345", "Random tag"}});
            
            //Act
            var rule = new ReleaseIsRelatedToChangeRequest(_client);
            var evaluatedRule = rule.Evaluate(_config.Project, ReleaseId);

            //Assert
            evaluatedRule.ShouldBeFalse();
        }
    }
}
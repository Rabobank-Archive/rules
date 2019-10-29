using System.Threading.Tasks;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class BuildPipelineHasSonarqubeTaskTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;

        public BuildPipelineHasSonarqubeTaskTests(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(_config.Organization, _config.Token);
        }

        [Fact]
        public async Task EvaluateBuildIntegrationTest()
        {
            var projectId = (await _client.GetAsync(Project.Properties(_config.Project))).Id;
            var buildPipeline = await _client.GetAsync(Builds.BuildDefinition(projectId, "2"))
                .ConfigureAwait(false);

            var rule = new BuildPipelineHasSonarqubeTask(_client);
            (await rule.EvaluateAsync(projectId, buildPipeline)).GetValueOrDefault().ShouldBeTrue();
        }
    }
}
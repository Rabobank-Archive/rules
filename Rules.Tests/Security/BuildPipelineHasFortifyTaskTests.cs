using System.Threading.Tasks;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class BuildPipelineHasFortifyTaskTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;

        public BuildPipelineHasFortifyTaskTests(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(_config.Organization, _config.Token);
        }

        [Fact]
        public async Task EvaluateBuildIntegrationTest_gui()
        {
            var project = await _client.GetAsync(Project.ProjectById(_config.Project));
            var buildPipeline = await _client.GetAsync(Builds.BuildDefinition(project.Id, "2"))
                .ConfigureAwait(false);

            var rule = new BuildPipelineHasFortifyTask(_client);
            (await rule.EvaluateAsync(project, buildPipeline)).GetValueOrDefault().ShouldBeFalse();
        }

        [Fact]
        public async Task EvaluateBuildIntegrationTest_yaml()
        {
            var project = await _client.GetAsync(Project.ProjectById(_config.Project));
            var buildPipeline = await _client.GetAsync(Builds.BuildDefinition(project.Id, "197"))
                .ConfigureAwait(false);

            var rule = new BuildPipelineHasFortifyTask(_client);
            (await rule.EvaluateAsync(project, buildPipeline)).GetValueOrDefault().ShouldBeFalse();
        }
    }
}
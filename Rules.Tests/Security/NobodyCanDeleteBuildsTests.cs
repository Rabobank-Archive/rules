using System.Threading.Tasks;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanDeleteBuildsTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;

        public NobodyCanDeleteBuildsTests(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(_config.Organization, _config.Token);
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateBuildIntegrationTest()
        {
            var project = await _client.GetAsync(Project.ProjectById(_config.Project));
            var buildPipeline = await _client.GetAsync(Builds.BuildDefinition(project.Id, "2"))
                .ConfigureAwait(false);

            var rule = new NobodyCanDeleteBuilds(_client);
            (await rule.EvaluateAsync(project, buildPipeline)).GetValueOrDefault().ShouldBeTrue();
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task ReconcileBuildIntegrationTest()
        {
            var projectId = (await _client.GetAsync(VstsService.Requests.Project.Properties(_config.Project))).Id;

            var rule = new NobodyCanDeleteBuilds(_client) as IReconcile;
            await rule.ReconcileAsync(projectId, "2");
        }
    }
}
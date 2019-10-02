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

        public NobodyCanDeleteBuildsTests(TestConfig config)
        {
            _config = config;
        }
        [Fact]
        public async Task EvaluateBuildIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(Project.Properties(_config.Project))).Id;
            var buildPipeline = await client.GetAsync(Builds.BuildDefinition(projectId, "2"))
                .ConfigureAwait(false);

            var rule = new NobodyCanDeleteBuilds(client);
            (await rule.EvaluateAsync(projectId, buildPipeline)).ShouldBeTrue();
        }

        [Fact]
        public async Task ReconcileBuildIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(VstsService.Requests.Project.Properties(_config.Project))).Id;

            var rule = new NobodyCanDeleteBuilds(client) as IReconcile;
            await rule.ReconcileAsync(projectId, "2");
        }
    }
}
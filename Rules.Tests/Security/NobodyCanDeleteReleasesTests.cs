using System.Threading.Tasks;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanDeleteReleasesTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public NobodyCanDeleteReleasesTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        public async Task EvaluateReleaseIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(Project.Properties(_config.Project))).Id;
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, "1"))
                .ConfigureAwait(false);

            var rule = new NobodyCanDeleteReleases(client);
            (await rule.EvaluateAsync(projectId, _config.stageId, releasePipeline)).ShouldBe(true);
        }

        [Fact]
        [Trait("type", "integration")]
        public async Task ReconcileReleaseIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(Project.Properties(_config.Project))).Id;

            var rule = new NobodyCanDeleteReleases(client) as IReconcile;
            await rule.ReconcileAsync(projectId, null, "1");
        }
    }
}
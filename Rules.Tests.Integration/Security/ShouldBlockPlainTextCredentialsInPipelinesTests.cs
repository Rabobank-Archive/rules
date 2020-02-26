using System.Threading.Tasks;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;

namespace Rules.Tests.Integration.Security
{
    public class ShouldBlockPlainTextCredentialsInPipelinesTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public ShouldBlockPlainTextCredentialsInPipelinesTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(SecurePipelineScan.VstsService.Requests.Project.Properties(_config.Project))).Id;

            var rule = new ShouldBlockPlainTextCredentialsInPipelines(client);
            (await rule.EvaluateAsync(projectId)).ShouldBeTrue();
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task ReconcileIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(SecurePipelineScan.VstsService.Requests.Project.Properties(_config.Project))).Id;

            var rule = new ShouldBlockPlainTextCredentialsInPipelines(client) as IProjectReconcile;
            await rule.ReconcileAsync(projectId);
        }
    }
}
using System.Threading.Tasks;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class ShouldBlockPlainTextCredentialsInPipelinesTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public ShouldBlockPlainTextCredentialsInPipelinesTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        public async Task EvaluateIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(VstsService.Requests.Project.Properties(_config.Project))).Id;

            var rule = new ShouldBlockPlainTextCredentialsInPipelines(client);
            (await rule.Evaluate(projectId)).ShouldBeTrue();
        }

        [Fact]
        public async Task ReconcileIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(VstsService.Requests.Project.Properties(_config.Project))).Id;

            var rule = new ShouldBlockPlainTextCredentialsInPipelines(client) as IProjectReconcile;
            await rule.Reconcile(projectId);
        }
    }
}
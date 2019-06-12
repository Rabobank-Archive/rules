using System.Threading.Tasks;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanDeleteBuildPipelinesTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public NobodyCanDeleteBuildPipelinesTests(TestConfig config)
        {
            _config = config;
        }
        [Fact]
        public async Task EvaluateBuildPipelineIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(VstsService.Requests.Project.Properties(_config.Project))).Id;

            var rule = new NobodyCanDeleteBuildPipelines(client);
            (await rule.Evaluate(projectId, "2")).ShouldBeTrue();
        }

        [Fact]
        public async Task ReconcileBuildPipelineIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(VstsService.Requests.Project.Properties(_config.Project))).Id;

            var rule = new NobodyCanDeleteBuildPipelines(client) as IReconcile;
            await rule.Reconcile(projectId, "2");
        }
    }
}
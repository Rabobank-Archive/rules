using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanDeleteReleasePipelinesTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public NobodyCanDeleteReleasePipelinesTests(TestConfig config)
        {
            _config = config;
        }
        [Fact]
        public void EvaluateReleasePipelineIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = new NobodyCanDeleteReleasePipelines(client);
            rule.Evaluate(projectId, "1").ShouldBeTrue();
        }

        [Fact]
        public void ReconcileReleasePipelineIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = new NobodyCanDeleteReleasePipelines(client) as IReconcile;
            rule.Reconcile(projectId, "1");
        }
    }
}
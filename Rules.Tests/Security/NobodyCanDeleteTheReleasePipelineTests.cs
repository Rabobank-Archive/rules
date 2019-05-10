using SecurePipelineScan.VstsService;
using SecurePipelineScan.Rules.Security;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanDeleteTheReleasePipelineTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private const string PipelineId = "1";

        public NobodyCanDeleteTheReleasePipelineTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        public void EvaluateIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = new NobodyCanDeleteTheReleasedPipeline(client);
            rule.Evaluate(projectId, PipelineId).ShouldBeTrue();
        }

        [Fact]
        public void ReconcileIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = new NobodyCanDeleteTheReleasedPipeline(client);
            rule.Reconcile(projectId, PipelineId);
        }
    }
}
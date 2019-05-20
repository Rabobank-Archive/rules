using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
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
        public void EvaluateBuildIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = new NobodyCanDeleteBuilds(client);
            rule.Evaluate(projectId, "2").ShouldBeTrue();
        }

        [Fact]
        public void ReconcileBuildIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = new NobodyCanDeleteBuilds(client) as IReconcile;
            rule.Reconcile(projectId, "2");
        }
    }
}
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanDeleteBuildPipelinesTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IRestClientFactory _factory;

        public NobodyCanDeleteBuildPipelinesTests(TestConfig config)
        {
            _config = config;
            _factory = new RestClientFactory();
        }
        [Fact]
        public void EvaluateBuildPipelineIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token, _factory);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = new NobodyCanDeleteBuildPipelines(client);
            rule.Evaluate(projectId, "2").ShouldBeTrue();
        }

        [Fact]
        public void ReconcileBuildPipelineIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token, _factory);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = new NobodyCanDeleteBuildPipelines(client) as IReconcile;
            rule.Reconcile(projectId, "2");
        }
    }
}
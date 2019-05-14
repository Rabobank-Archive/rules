using SecurePipelineScan.VstsService;
using SecurePipelineScan.Rules.Security;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanDeleteThisPipelineBaseTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public NobodyCanDeleteThisPipelineBaseTests(TestConfig config)
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

        [Fact]
        public void EvaluateBuildPipelineIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = new NobodyCanDeleteBuildPipelines(client);
            rule.Evaluate(projectId, "2").ShouldBeTrue();
        }

        [Fact]
        public void ReconcileBuildPipelineIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = new NobodyCanDeleteBuildPipelines(client) as IReconcile;
            rule.Reconcile(projectId, "2");
        }

        [Fact]
        public void EvaluateReleaseIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = new NobodyCanDeleteReleases(client);
            rule.Evaluate(projectId, "1").ShouldBeTrue();
        }

        [Fact]
        public void ReconcileReleaseIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = new NobodyCanDeleteReleases(client) as IReconcile;
            rule.Reconcile(projectId, "1");
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
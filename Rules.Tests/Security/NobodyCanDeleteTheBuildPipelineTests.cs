using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.VstsService;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;
using SecurityNamespace = SecurePipelineScan.VstsService.Response.SecurityNamespace;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanDeleteTheBuildPipelineTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private const string BuildPipelineId = "2";

        public NobodyCanDeleteTheBuildPipelineTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        public void EvaluateIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = new NobodyCanDeleteTheBuildPipeline(client);
            rule.Evaluate(projectId, BuildPipelineId).ShouldBeTrue();
        }

        [Fact]
        public void ReconcileIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = new NobodyCanDeleteTheBuildPipeline(client);
            rule.Reconcile(projectId, BuildPipelineId);
        }
    }
}
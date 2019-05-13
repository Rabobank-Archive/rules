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
    public class NobodyCanDeleteThePipelineTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public NobodyCanDeleteThePipelineTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        public void EvaluateBuildIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = NobodyCanDeleteThePipeline.Build(client);
            rule.Evaluate(projectId, "2").ShouldBeTrue();
        }

        [Fact]
        public void ReconcileBuildIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = NobodyCanDeleteThePipeline.Build(client) as IReconcile;
            rule.Reconcile(projectId, "2");
        }

        [Fact]
        public void EvaluateBuildPipelineIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = NobodyCanDeleteThePipeline.BuildPipeline(client);
            rule.Evaluate(projectId, "2").ShouldBeTrue();
        }

        [Fact]
        public void ReconcileBuildPipelineIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = NobodyCanDeleteThePipeline.BuildPipeline(client) as IReconcile;
            rule.Reconcile(projectId, "2");
        }
        
        [Fact]
        public void EvaluateReleaseIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = NobodyCanDeleteThePipeline.Release(client);
            rule.Evaluate(projectId, "1").ShouldBeTrue();
        }

        [Fact]
        public void ReconcileReleaseIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = NobodyCanDeleteThePipeline.Release(client) as IReconcile;
            rule.Reconcile(projectId, "1");
        }

        [Fact]
        public void EvaluateReleasePipelineIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = NobodyCanDeleteThePipeline.ReleasePipeline(client);
            rule.Evaluate(projectId, "1").ShouldBeTrue();
        }

        [Fact]
        public void ReconcileReleasePipelineIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = client.Get(VstsService.Requests.Project.Properties(_config.Project)).Id;

            var rule = NobodyCanDeleteThePipeline.ReleasePipeline(client) as IReconcile;
            rule.Reconcile(projectId, "1");
        }
    }
}
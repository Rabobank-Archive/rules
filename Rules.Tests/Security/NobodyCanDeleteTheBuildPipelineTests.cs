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
            var rule = new NobodyCanDeleteTheBuildPipeline(new VstsRestClient(_config.Organization, _config.Token));
            rule.Evaluate(_config.Project, BuildPipelineId).ShouldBeTrue();
        }
    }
}
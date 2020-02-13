using System;
using System.Threading.Tasks;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Permissions;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanDeleteBuildsTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;

        public NobodyCanDeleteBuildsTests(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(_config.Organization, _config.Token);
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task ReconcileIntegrationTest()
        {
            var project = await _client.GetAsync(Project.ProjectById(_config.Project));
            var buildPipeline = await _client.GetAsync(Builds.BuildDefinition(project.Id, "2"))
                .ConfigureAwait(false);

            await ManagePermissions
                .ForBuildPipeline(_client, project.Id, buildPipeline.Id, buildPipeline.Path)
                .Permissions(8)
                .SetToAsync(PermissionId.Allow);

            var rule = new NobodyCanDeleteBuilds(_client);
            (await rule.EvaluateAsync(project, buildPipeline))
                .ShouldBe(false);

            await rule.ReconcileAsync(project.Id, buildPipeline.Id);
            await Task.Delay(TimeSpan.FromSeconds(10));

            (await rule.EvaluateAsync(project, buildPipeline))
                .ShouldBe(true);
        }
    }
}
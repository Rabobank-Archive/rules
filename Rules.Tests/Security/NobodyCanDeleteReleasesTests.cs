using System;
using System.Threading.Tasks;
using NSubstitute;
using SecurePipelineScan.Rules.Permissions;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanDeleteReleasesTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public NobodyCanDeleteReleasesTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task ReconcileIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(Project.Properties(_config.Project))).Id;
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, "1"))
                .ConfigureAwait(false);

            await ManagePermissions
                .ForReleasePipeline(client, projectId, releasePipeline.Id, releasePipeline.Path)
                .Permissions(4)
                .SetToAsync(PermissionId.Allow);

            var rule = new NobodyCanDeleteReleases(client);
            (await rule.EvaluateAsync(projectId, releasePipeline))
                .ShouldBe(false);

            await rule.ReconcileAsync(projectId, releasePipeline.Id);
            await Task.Delay(TimeSpan.FromSeconds(10));

            (await rule.EvaluateAsync(projectId, releasePipeline))
                .ShouldBe(true);
        }
    }
}
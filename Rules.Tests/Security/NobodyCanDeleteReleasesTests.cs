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
        public async Task EvaluateReleaseIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(Project.Properties(_config.Project))).Id;
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, "1"))
                .ConfigureAwait(false);

            var rule = new NobodyCanDeleteReleases(client);
            (await rule.EvaluateAsync(projectId, _config.stageId, releasePipeline)).ShouldBe(true);
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
                .ForReleasePipeline(client, projectId, releasePipeline.Id)
                .Permissions(4)
                .SetToAsync(PermissionId.Allow);

            var rule = new NobodyCanDeleteReleases(client);
            (await rule.EvaluateAsync(projectId, _config.stageId, releasePipeline))
                .ShouldBe(false);

            await rule.ReconcileAsync(projectId, releasePipeline.Id, null, null);
            await Task.Delay(TimeSpan.FromSeconds(2));

            (await rule.EvaluateAsync(projectId, _config.stageId, releasePipeline))
                .ShouldBe(true);
        }

        [Fact]
        public void RequiresStageId_ShouldBeFalse()
        {
            var client = Substitute.For<IVstsRestClient>();
            var rule = new NobodyCanDeleteReleases(client) as IReconcile;
            rule.RequiresStageId.ShouldBe(false);
        }
    }
}
using System;
using Polly;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Permissions;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;
using Project = SecurePipelineScan.VstsService.Requests.Project;
using Task = System.Threading.Tasks.Task;

namespace Rules.Tests.Integration.Security
{
    public class NobodyCanManageApprovalsAndCreateReleasesTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public NobodyCanManageApprovalsAndCreateReleasesTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateAndReconcileIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(Project.Properties(_config.Project))).Id;
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, "1"))
                .ConfigureAwait(false);

            await ManagePermissions
                .ForReleasePipeline(client, projectId, releasePipeline.Id, releasePipeline.Path)
                .For("Contributors")
                .Permissions(8, 64)
                .SetToAsync(PermissionId.Allow);

            var rule = new NobodyCanManageApprovalsAndCreateReleases(client);
            await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(Constants.NumRetries, t => TimeSpan.FromSeconds(t))
                .ExecuteAsync(async () =>
            {
                (await rule.EvaluateAsync(projectId, releasePipeline))
                .ShouldBe(false);
            });

            await rule.ReconcileAsync(projectId, releasePipeline.Id);
            
            await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(Constants.NumRetries, t => TimeSpan.FromSeconds(t))
                .ExecuteAsync(async () =>
                {
                    (await rule.EvaluateAsync(projectId, releasePipeline)).ShouldBe(true);
                });
        }
    }
}
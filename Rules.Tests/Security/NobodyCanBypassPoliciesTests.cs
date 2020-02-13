using System;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.Rules.Security;
using Shouldly;
using Xunit;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Permissions;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanBypassPoliciesTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private const int PermissionBitBypassPoliciesPullRequest = 32768;

        public NobodyCanBypassPoliciesTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task ReconcileIntegrationTest()
        {
            var repositoryId = "3167b64e-c72b-4c55-84eb-986ac62d0dec";
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(Project.Properties(_config.Project))).Id;

            await ManagePermissions
                .ForRepository(client, projectId, repositoryId)
                .Permissions(PermissionBitBypassPoliciesPullRequest)
                .SetToAsync(PermissionId.Allow);

            var rule = new NobodyCanBypassPolicies(client);
            (await rule.EvaluateAsync(projectId, repositoryId))
                .ShouldBe(false);
            await rule.ReconcileAsync(projectId, repositoryId);
            await Task.Delay(TimeSpan.FromSeconds(10));
            (await rule.EvaluateAsync(projectId, repositoryId))
                .ShouldBe(true);
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task ReconcileIntegrationTestForMasterBranchPermission()
        {
            var repositoryId = "6435e3f0-15b7-4302-814d-4ab586e61f8b";
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(Project.Properties(_config.Project))).Id;

            await ManagePermissions
                .ForMasterBranch(client, projectId, repositoryId)
                .Permissions(PermissionBitBypassPoliciesPullRequest)
                .SetToAsync(PermissionId.Allow);

            var rule = new NobodyCanBypassPolicies(client);
            (await rule.EvaluateAsync(projectId, repositoryId))
                .ShouldBe(false);
            await rule.ReconcileAsync(projectId, repositoryId);
            await Task.Delay(TimeSpan.FromSeconds(10));
            (await rule.EvaluateAsync(projectId, repositoryId))
                .ShouldBe(true);
        }
    }
}
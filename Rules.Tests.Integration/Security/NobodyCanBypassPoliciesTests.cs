using System;
using System.Threading.Tasks;
using Polly;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Permissions;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;

namespace Rules.Tests.Integration.Security
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
            await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(Constants.NumRetries, t => TimeSpan.FromSeconds(t))
                .ExecuteAsync(async () =>
            {
                    (await rule.EvaluateAsync(projectId, repositoryId)).ShouldBe(true);
            });
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
            await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(Constants.NumRetries, t => TimeSpan.FromSeconds(t))
                .ExecuteAsync(async () =>
            {
                    (await rule.EvaluateAsync(projectId, repositoryId)).ShouldBe(true);
            });
        }
    }
}
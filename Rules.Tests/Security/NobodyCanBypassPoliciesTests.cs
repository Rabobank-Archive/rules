using System;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.Rules.Permissions;
using SecurePipelineScan.Rules.Security;
using Shouldly;
using Xunit;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanBypassPoliciesTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private const string RepositoryId = "3167b64e-c72b-4c55-84eb-986ac62d0dec";
        private const int PermissionBitBypassPoliciesPullRequest = 32768;

        public NobodyCanBypassPoliciesTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(Project.Properties(_config.Project))).Id;

            var rule = new NobodyCanBypassPolicies(client);
            (await rule.EvaluateAsync(projectId, RepositoryId, null)).ShouldBeTrue();
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task ReconcileIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(Project.Properties(_config.Project))).Id;

            await ManagePermissions
                .ForRepository(client, projectId, RepositoryId)
                .Permissions(PermissionBitBypassPoliciesPullRequest)
                .SetToAsync(PermissionId.Allow);
            
            var rule = new NobodyCanBypassPolicies(client);
            (await rule.EvaluateAsync(projectId, RepositoryId, null))
                .ShouldBe(false);
            await rule.ReconcileAsync(projectId, RepositoryId, null);
            await Task.Delay(TimeSpan.FromSeconds(2));
            (await rule.EvaluateAsync(projectId, RepositoryId, null))
                .ShouldBe(true);
        }
    }
}
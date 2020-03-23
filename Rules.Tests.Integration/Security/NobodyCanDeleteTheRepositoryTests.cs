using System;
using System.Threading.Tasks;
using Polly;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Permissions;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;
using Permissions = SecurePipelineScan.Rules.PermissionBits.Repository;

namespace Rules.Tests.Integration.Security
{
    public class NobodyCanDeleteTheRepositoryTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private const string RepositoryId = "3167b64e-c72b-4c55-84eb-986ac62d0dec";


        public NobodyCanDeleteTheRepositoryTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(Project.Properties(_config.Project))).Id;

            var rule = new NobodyCanDeleteTheRepository(client);
            (await rule.EvaluateAsync(projectId, RepositoryId)).ShouldBe(true);
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task ReconcileIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(Project.Properties(_config.Project))).Id;

            await ManagePermissions
                .ForRepository(client, projectId, RepositoryId)
                .Permissions(Permissions.DeleteRepository)
                .SetToAsync(PermissionId.Allow);

            var rule = new NobodyCanDeleteTheRepository(client);
            (await rule.EvaluateAsync(projectId, RepositoryId))
                .ShouldBe(false);

            await rule.ReconcileAsync(projectId, RepositoryId);
            await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(Constants.NumRetries, t => TimeSpan.FromSeconds(t))
                .ExecuteAsync(async () =>
            {
                    (await rule.EvaluateAsync(projectId, RepositoryId)).ShouldBe(true);
            });
        }
    }
}
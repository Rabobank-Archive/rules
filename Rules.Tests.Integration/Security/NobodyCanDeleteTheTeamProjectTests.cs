using System;
using Polly;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Permissions;
using Shouldly;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Rules.Tests.Integration.Security
{
    public class NobodyCanDeleteTheTeamProjectTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public NobodyCanDeleteTheTeamProjectTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task FixIntegrationTest()
        {
            var rule = new NobodyCanDeleteTheTeamProject(new VstsRestClient(_config.Organization, _config.Token));
            await rule.ReconcileAsync(_config.Project);
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task ReconcileIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);

            await ManagePermissions
                .ForTeamProject(client, _config.Project)
                .Permissions((4, "52d39943-cb85-4d7f-8fa8-c6baac873819"))
                .SetToAsync(PermissionId.Allow);

            var rule = new NobodyCanDeleteTheTeamProject(client);
            (await rule.EvaluateAsync(_config.Project))
                .ShouldBe(false);

            await rule.ReconcileAsync(_config.Project);
            await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(Constants.NumRetries, t => TimeSpan.FromSeconds(t))
                .ExecuteAsync(async () =>
                {
                    (await rule.EvaluateAsync(_config.Project)).ShouldBe(true);
                });
        }
    }
}
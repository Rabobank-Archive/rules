using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using Polly;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;

namespace Rules.Tests.Integration.Security
{
    public class ReleaseBranchesProtectedByPoliciesTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private const string RepositoryId = "3167b64e-c72b-4c55-84eb-986ac62d0dec";
        private readonly IPoliciesResolver _policiesResolver = Substitute.For<IPoliciesResolver>();

        public ReleaseBranchesProtectedByPoliciesTests(TestConfig config) => _config = config;

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateAndReconcileIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            SetupPoliciesResolver(_policiesResolver, client, _config.Project);
            var rule = new ReleaseBranchesProtectedByPolicies(client, _policiesResolver);

            var policy = client.Get(Policies.MinimumNumberOfReviewersPolicies(_config.Project))
                .FirstOrDefault(p => p.Settings.Scope
                    .Any(s => s.RepositoryId?.ToString() == RepositoryId && s.RefName == "refs/heads/master"));
            if (policy != default)
                await client.DeleteAsync(Policies.Policy(_config.Project, policy.Id));

            await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(Constants.NumRetries, t => TimeSpan.FromSeconds(t))
                .ExecuteAsync(async () =>
                {
                    (await rule.EvaluateAsync(_config.Project, RepositoryId)).ShouldBe(false);
                });

            await rule.ReconcileAsync(_config.Project, RepositoryId);

            await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(Constants.NumRetries, t => TimeSpan.FromSeconds(t))
                .ExecuteAsync(async () =>
                {
                    (await rule.EvaluateAsync(_config.Project, RepositoryId)).ShouldBe(true);
                });
        }

        private static void SetupPoliciesResolver(IPoliciesResolver resolver, IVstsRestClient client, string projectId) => 
            resolver
                .Resolve(projectId)
                .Returns(client.Get(Policies.MinimumNumberOfReviewersPolicies(projectId)));
    }
}
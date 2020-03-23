using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Xunit;

namespace AzureDevOps.Compliance.Rules.Tests.Integration
{
    public class ReleaseBranchesProtectedByPoliciesTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private const string RepositoryId = "3167b64e-c72b-4c55-84eb-986ac62d0dec";
        private readonly Fixture _fixture = new Fixture {RepeatCount = 1};

        public ReleaseBranchesProtectedByPoliciesTests(TestConfig config)
        {
            _config = config;
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task EvaluateIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(Project.Properties(_config.Project))).Id;

            var rule = new ReleaseBranchesProtectedByPolicies(client, Substitute.For<IPoliciesResolver>());
            await rule.EvaluateAsync(projectId, RepositoryId);
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task Reconcile()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var resolver = Substitute.For<IPoliciesResolver>();
            var policies = client.Get(Policies.MinimumNumberOfReviewersPolicies(_config.Project)).ToList();
            
            resolver.Resolve(_config.Project)
                .Returns(policies);
            
            var rule = new ReleaseBranchesProtectedByPolicies(client, resolver) as IReconcile;
            await rule.ReconcileAsync(_config.Project, RepositoryId);
        }
    }
}
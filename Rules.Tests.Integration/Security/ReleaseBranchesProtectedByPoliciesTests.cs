using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
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
        private readonly Fixture _fixture = new Fixture {RepeatCount = 1};
        private readonly IPoliciesResolver _policiesResolver = Substitute.For<IPoliciesResolver>();

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
            SetupPoliciesResolver(_policiesResolver, client, projectId); 
            
            var rule = new ReleaseBranchesProtectedByPolicies(client, _policiesResolver);
            var result = await rule.EvaluateAsync(projectId, RepositoryId);

            result.ShouldBe(true);
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task Reconcile()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            SetupPoliciesResolver(_policiesResolver, client, _config.Project);

            var rule = new ReleaseBranchesProtectedByPolicies(client, _policiesResolver) as IReconcile;
            await rule.ReconcileAsync(_config.Project, RepositoryId);
        }

        private static void SetupPoliciesResolver(IPoliciesResolver resolver, IVstsRestClient client, string projectId) => 
            resolver
                .Resolve(projectId)
                .Returns(client.Get(Policies.MinimumNumberOfReviewersPolicies(projectId)));
    }
}
using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using Requests = SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class ReleaseBranchesProtectedByPoliciesTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private const string RepositoryId = "3167b64e-c72b-4c55-84eb-986ac62d0dec";
        private readonly Fixture _fixture = new Fixture { RepeatCount = 1 };
        private readonly IVstsRestClient _client = Substitute.For<IVstsRestClient>();

        public ReleaseBranchesProtectedByPoliciesTests(TestConfig config)
        {
            _config = config;
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        public async Task EvaluateIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(Requests.Project.Properties(_config.Project))).Id;
            var policies = client.Get(Requests.Policies.MinimumNumberOfReviewersPolicies(projectId));

            var rule = new ReleaseBranchesProtectedByPolicies(client);
            await rule.EvaluateAsync(projectId, RepositoryId, policies);
        }

        [Fact]
        public async Task EvaluateShouldReturnTrueForRepoHasCorrectPolicies()
        {
            //Arrange
            CustomizeScope(_fixture, RepositoryId);
            CustomizeMinimumNumberOfReviewersPolicy(_fixture);
            CustomizePolicySettings(_fixture);

            SetupClient(_client, _fixture);
            var policies = new List<MinimumNumberOfReviewersPolicy>
                { _fixture.Create<MinimumNumberOfReviewersPolicy>() };

            //Act
            var rule = new ReleaseBranchesProtectedByPolicies(_client);
            var evaluatedRule = await rule.EvaluateAsync(_config.Project, RepositoryId, policies);

            //Assert
            evaluatedRule.ShouldBeTrue();
        }

        [Fact]
        public async Task EvaluateShouldReturnFalseForRepoNotMatchingPolicies()
        {
            //Arrange
            SetupClient(_client, _fixture);
            var policies = new List<MinimumNumberOfReviewersPolicy>
                { _fixture.Create<MinimumNumberOfReviewersPolicy>() };

            //Act
            var rule = new ReleaseBranchesProtectedByPolicies(_client);
            var evaluatedRule = await rule.EvaluateAsync(_config.Project, RepositoryId, policies);

            //Assert
            evaluatedRule.ShouldBeFalse();
        }

        [Fact]
        public async Task EvaluateShouldReturnFalseWhenMinimumApproverCountIsLessThan2()
        {
            //Arrange
            CustomizeScope(_fixture, RepositoryId);
            // ReSharper disable once RedundantArgumentDefaultValue
            CustomizeMinimumNumberOfReviewersPolicy(_fixture, true);
            // ReSharper disable once ArgumentsStyleLiteral
            CustomizePolicySettings(_fixture, minimumApproverCount: 1);

            SetupClient(_client, _fixture);
            var policies = new List<MinimumNumberOfReviewersPolicy>
                { _fixture.Create<MinimumNumberOfReviewersPolicy>() };

            //Act
            var rule = new ReleaseBranchesProtectedByPolicies(_client);
            var evaluatedRule = await rule.EvaluateAsync(_config.Project, RepositoryId, policies);

            //Assert
            evaluatedRule.ShouldBeFalse();
        }

        [Fact]
        public async Task EvaluateShouldReturnFalseWhenPolicyIsNotEnabled()
        {
            //Arrange
            CustomizeScope(_fixture, RepositoryId);
            CustomizeMinimumNumberOfReviewersPolicy(_fixture, false);
            CustomizePolicySettings(_fixture);

            SetupClient(_client, _fixture);
            var policies = new List<MinimumNumberOfReviewersPolicy>
                { _fixture.Create<MinimumNumberOfReviewersPolicy>() };

            //Act
            var rule = new ReleaseBranchesProtectedByPolicies(_client);
            var evaluatedRule = await rule.EvaluateAsync(_config.Project, RepositoryId, policies);

            //Assert
            evaluatedRule.ShouldBeFalse();
        }

        [Fact]
        public async Task EvaluateShouldReturnFalseWhenPolicyIsNotBlocking()
        {
            //Arrange
            CustomizeScope(_fixture, RepositoryId);
            CustomizeMinimumNumberOfReviewersPolicy(_fixture, isBlocking: false);
            CustomizePolicySettings(_fixture);

            SetupClient(_client, _fixture);
            var policies = new List<MinimumNumberOfReviewersPolicy>
                { _fixture.Create<MinimumNumberOfReviewersPolicy>() };

            //Act
            var rule = new ReleaseBranchesProtectedByPolicies(_client);
            var evaluatedRule = await rule.EvaluateAsync(_config.Project, RepositoryId, policies);

            //Assert
            evaluatedRule.ShouldBeFalse();
        }

        [Fact]
        public async Task EvaluateShouldReturnFalseWhenThereAreNoCorrectPoliciesForMasterBranch()
        {
            //Arrange
            CustomizeScope(_fixture, refName: "ref/heads/not-master");
            CustomizeMinimumNumberOfReviewersPolicy(_fixture);
            CustomizePolicySettings(_fixture);

            SetupClient(_client, _fixture);
            var policies = new List<MinimumNumberOfReviewersPolicy>
                { _fixture.Create<MinimumNumberOfReviewersPolicy>() };

            //Act
            var rule = new ReleaseBranchesProtectedByPolicies(_client);
            var evaluatedRule = await rule.EvaluateAsync(_config.Project, RepositoryId, policies);

            //Assert
            evaluatedRule.ShouldBeFalse();
        }

        [Fact]
        public void Reconcile()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var rule = new ReleaseBranchesProtectedByPolicies(client) as IReconcile;
            rule.ReconcileAsync(_config.Project, RepositoryId, RuleScopes.Repositories, null);
        }

        [Fact]
        public async Task GivenNoPolicyPresent_WhenReconcile_PostIsUsed()
        {
            // Arrange
            CustomizeScope(_fixture, refName: "some/other/branch");
            SetupClient(_client, _fixture);

            // Act
            var rule = new ReleaseBranchesProtectedByPolicies(_client) as IReconcile;
            await rule.ReconcileAsync(_config.Project, RepositoryId, RuleScopes.Repositories, null);

            // Assert
            await _client
                .Received()
                .PostAsync(Arg.Any<IVstsRequest<Policy, Policy>>(), Arg.Any<MinimumNumberOfReviewersPolicy>());
        }

        [Fact]
        public async Task PolicySettingsOnReconcile()
        {
            // Arrange
            CustomizeScope(_fixture);
            SetupClient(_client, _fixture);

            // Act
            var rule = new ReleaseBranchesProtectedByPolicies(_client) as IReconcile;
            await rule.ReconcileAsync(_config.Project, RepositoryId, RuleScopes.Repositories, null);

            // Assert
            await _client
                .Received()
                .PutAsync(Arg.Any<IVstsRequest<Policy, Policy>>(), Arg.Is<MinimumNumberOfReviewersPolicy>(p => !p.IsDeleted && p.IsBlocking && p.IsEnabled));
        }

        [Fact]
        public async Task GivenExistingPolicyPresent_WhenReconcile_PutIsUsed()
        {
            // Arrange
            CustomizeScope(_fixture);
            SetupClient(_client, _fixture);

            // Act
            var rule = new ReleaseBranchesProtectedByPolicies(_client) as IReconcile;
            await rule.ReconcileAsync(_config.Project, RepositoryId, RuleScopes.Repositories, null);

            // Assert
            await _client
                .Received()
                .PutAsync(Arg.Any<IVstsRequest<Policy>>(), Arg.Any<MinimumNumberOfReviewersPolicy>());
        }

        [Fact]
        public async Task GivenExistingPolicyHasApproverCount_WhenReconcile_NotUpdated()
        {
            // Arrange
            CustomizeScope(_fixture);
            CustomizePolicySettings(_fixture, 3);
            SetupClient(_client, _fixture);

            // Act
            var rule = new ReleaseBranchesProtectedByPolicies(_client) as IReconcile;
            await rule.ReconcileAsync(_config.Project, RepositoryId, RuleScopes.Repositories, null);

            // Assert
            await _client
                .Received()
                .PutAsync(Arg.Any<IVstsRequest<Policy>>(), Arg.Is<MinimumNumberOfReviewersPolicy>(x => x.Settings.MinimumApproverCount == 3));
        }

        [Fact]
        public void RequiresStageId_ShouldBeFalse()
        {
            var client = Substitute.For<IVstsRestClient>();
            var rule = new ReleaseBranchesProtectedByPolicies(client) as IReconcile;
            rule.RequiresStageId.ShouldBe(false);
        }

        private static void CustomizeScope(IFixture fixture,
            string id = null,
            string refName = "refs/heads/master")
        {
            fixture.Customize<Scope>(ctx => ctx
                .With(r => r.RepositoryId, new Guid(id ?? RepositoryId))
                .With(r => r.RefName, refName));
        }

        private static void CustomizePolicySettings(IFixture fixture,
            int minimumApproverCount = 2,
            bool resetOnSourcePush = true,
            bool creatorVoteCounts = true)
        {
            fixture.Customize<MinimumNumberOfReviewersPolicySettings>(ctx => ctx
                .With(r => r.MinimumApproverCount, minimumApproverCount)
                .With(r => r.ResetOnSourcePush, resetOnSourcePush)
                .With(r => r.CreatorVoteCounts, creatorVoteCounts));
        }

        private static void CustomizeMinimumNumberOfReviewersPolicy(IFixture fixture,
            bool isBlocking = true,
            bool isDeleted = true,
            bool isEnabled = true)
        {
            fixture.Customize<MinimumNumberOfReviewersPolicy>(ctx => ctx
                .With(r => r.IsEnabled, isEnabled)
                .With(r => r.IsDeleted, isDeleted)
                .With(r => r.IsBlocking, isBlocking));
        }


        private static void SetupClient(IVstsRestClient client, IFixture fixture)
        {
            client
                .Get(Arg.Any<IEnumerableRequest<MinimumNumberOfReviewersPolicy>>())
                .Returns(fixture.CreateMany<MinimumNumberOfReviewersPolicy>());
        }
    }
}
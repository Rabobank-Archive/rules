using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using Repository = SecurePipelineScan.VstsService.Response.Repository;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class ReleaseBranchesProtectedByPoliciesTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly Guid _id = new Guid( "3167b64e-c72b-4c55-84eb-986ac62d0dec");
        private readonly Fixture _fixture = new Fixture { RepeatCount = 1 };
        private readonly IVstsRestClient _client = Substitute.For<IVstsRestClient>();

        public ReleaseBranchesProtectedByPoliciesTests(TestConfig config)
        {
            _config = config;
            _fixture.Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        public void EvaluateShouldReturnTrueForRepoHasCorrectPolicies()
        {
            //Arrange
            CustomizeRepository(_fixture);
            CustomizeScope(_fixture, _id);
            CustomizeMinimumNumberOfReviewersPolicy(_fixture);
            CustomizePolicySettings(_fixture);

            SetupClient(_client, _fixture);

            //Act
            var rule = new ReleaseBranchesProtectedByPolicies(_client);
            var evaluatedRule = rule.Evaluate(_config.Project, "SOx-Compliant-Demo");

            //Assert
            evaluatedRule.ShouldBeTrue();
        }

        [Fact]
        public void EvaluateShouldReturnFalseForRepoNotMatchingPolicies()
        {
            //Arrange
            CustomizeRepository(_fixture);
            SetupClient(_client, _fixture);

            //Act
            var rule = new ReleaseBranchesProtectedByPolicies(_client);
            var evaluatedRule = rule.Evaluate(_config.Project, "SOx-Compliant-Demo");

            //Assert
            evaluatedRule.ShouldBeFalse();
        }

        [Fact]
        public void EvaluateShouldReturnFalseWhenMinimumApproverCountIsLessThan2()
        {
            //Arrange
            CustomizeRepository(_fixture);
            CustomizeScope(_fixture, _id);
            CustomizeMinimumNumberOfReviewersPolicy(_fixture, true);
            CustomizePolicySettings(_fixture, minimumApproverCount: 1);

            SetupClient(_client, _fixture);

            //Act
            var rule = new ReleaseBranchesProtectedByPolicies(_client);
            var evaluatedRule = rule.Evaluate(_config.Project, "SOx-Compliant-Demo");

            //Assert
            evaluatedRule.ShouldBeFalse();
        }

        [Fact]
        public void EvaluateShouldReturnFalseWhenPolicyIsNotEnabled()
        {
            //Arrange
            CustomizeRepository(_fixture);
            CustomizeScope(_fixture, _id);
            CustomizeMinimumNumberOfReviewersPolicy(_fixture, false);
            CustomizePolicySettings(_fixture);

            SetupClient(_client, _fixture);

            //Act
            var rule = new ReleaseBranchesProtectedByPolicies(_client);
            var evaluatedRule = rule.Evaluate(_config.Project, "SOx-Compliant-Demo");

            //Assert
            evaluatedRule.ShouldBeFalse();
        }

        [Fact]
        public void EvaluateShouldReturnFalseWhenThereAreNoCorrectPoliciesForMasterBranch()
        {
            //Arrange
            CustomizeRepository(_fixture);
            CustomizeScope(_fixture, refName: "ref/heads/not-master");
            CustomizeMinimumNumberOfReviewersPolicy(_fixture);
            CustomizePolicySettings(_fixture);

            SetupClient(_client, _fixture);

            //Act
            var rule = new ReleaseBranchesProtectedByPolicies(_client);
            var evaluatedRule = rule.Evaluate(_config.Project, "SOx-Compliant-Demo");

            //Assert
            evaluatedRule.ShouldBeFalse();
        }

        private void CustomizeRepository(IFixture fixture)
        {
            fixture.Customize<Repository>(ctx => ctx
                .With(r => r.Name, "SOx-Compliant-Demo")
                .With(r => r.Id, _id.ToString()));
        }

        private void CustomizeScope(IFixture fixture, 
            Guid? id = null,
            string refName = "refs/heads/master")
        {
            fixture.Customize<Scope>(ctx => ctx
                .With(r => r.RepositoryId, id ?? _id)
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

        private static void CustomizeMinimumNumberOfReviewersPolicy(IFixture fixture, bool enabled = true)
        {
            fixture.Customize<MinimumNumberOfReviewersPolicy>(ctx => ctx
                .With(r => r.IsEnabled, enabled));
        }


        private static void SetupClient(IVstsRestClient client, IFixture fixture)
        {
            client
                .Get(Arg.Any<IVstsRestRequest<Multiple<Repository>>>())
                .Returns(fixture.Create<Multiple<Repository>>());

            client
                .Get(Arg.Any<IVstsRestRequest<Multiple<MinimumNumberOfReviewersPolicy>>>())
                .Returns(fixture.Create<Multiple<MinimumNumberOfReviewersPolicy>>());
        }
    }
}
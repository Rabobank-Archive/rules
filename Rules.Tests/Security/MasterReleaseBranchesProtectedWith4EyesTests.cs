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
    public class MasterReleaseBranchesProtectedWith4EyesTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly string repoSoxCompliantDemo = "3167b64e-c72b-4c55-84eb-986ac62d0dec";
        private readonly Guid _id;
        private readonly Fixture _fixture;
        private readonly IVstsRestClient _client;

        public MasterReleaseBranchesProtectedWith4EyesTests(TestConfig config)
        {
            _config = config;
            _id = Guid.NewGuid();
            _fixture = new Fixture {RepeatCount = 1};
            _fixture.Customize(new AutoNSubstituteCustomization());
            _client = Substitute.For<IVstsRestClient>();
        }

        [Fact]
        public void EvaluateShouldReturnFalseForRepoNotMatchingPolicies()
        {
            //Arrange
            CustomizeRepository(_fixture, _id);
            var rule = new MasterAndReleaseBranchesProtected(_client);
            InitializeData(_client, _fixture);

            //Act
            var evaluatedRule = rule.Evaluate(_config.Project, repoSoxCompliantDemo);

            //Assert
            evaluatedRule.ShouldBeFalse();
        }

        [Fact]
        public void EvaluateShouldReturnTrueForRepoHavingCorrectPolicies()
        {
            //Arrange
            CustomizeRepository(_fixture, _id);
            CustomizeScope(_fixture, _id, "refs/heads/master");
            CustomizeMinimumNumberOfReviewersPolicy(_fixture);
            CustomizePolicySettings(_fixture);

            InitializeData(_client, _fixture);
            var rule = new MasterAndReleaseBranchesProtected(_client);

            //Act
            var evaluatedRule = rule.Evaluate(_config.Project, repoSoxCompliantDemo);

            //Assert
            evaluatedRule.ShouldBeTrue();
        }

        [Fact]
        public void EvaluateShouldReturnFalseWhenScopeIsNotMasterMinimumApproverCountIsLessThan2()
        {
            //Arrange
            CustomizeRepository(_fixture, _id);
            CustomizeScope(_fixture, _id);
            CustomizeMinimumNumberOfReviewersPolicy(_fixture, true);
            CustomizePolicySettings(_fixture, minimumApproverCount: 1);

            InitializeData(_client, _fixture);
            var rule = new MasterAndReleaseBranchesProtected(_client);

            //Act
            var evaluatedRule = rule.Evaluate(_config.Project, repoSoxCompliantDemo);

            //Assert
            evaluatedRule.ShouldBeFalse();
        }

        [Fact]
        public void EvaluateShouldReturnFalseWhenMinimumNumberOfReviewerPolicyIsFalse()
        {
            //Arrange
            CustomizeRepository(_fixture, _id);
            CustomizeScope(_fixture, _id);
            CustomizeMinimumNumberOfReviewersPolicy(_fixture, false);
            CustomizePolicySettings(_fixture, minimumApproverCount: 2);

            InitializeData(_client, _fixture);
            var rule = new MasterAndReleaseBranchesProtected(_client);

            //Act
            var evaluatedRule = rule.Evaluate(_config.Project, repoSoxCompliantDemo);

            //Assert
            evaluatedRule.ShouldBeFalse();
        }

        [Fact]
        public void EvaluateShouldReturnFalseWhenThereAreNoCorrectPoliciesForMasterBranch()
        {
            //Arrange
            CustomizeRepository(_fixture, _id);
            CustomizeScope(_fixture);
            CustomizeMinimumNumberOfReviewersPolicy(_fixture, true);
            CustomizePolicySettings(_fixture, 2, true, true);

            InitializeData(_client, _fixture);
            var rule = new MasterAndReleaseBranchesProtected(_client);

            //Act
            var evaluatedRule = rule.Evaluate(_config.Project, repoSoxCompliantDemo);

            //Assert
            evaluatedRule.ShouldBeFalse();
        }

        private void CustomizeRepository(Fixture fixture, Guid guid = default(Guid))
        {
            fixture.Customize<VstsService.Response.Repository>(ctx => ctx
                .With(r => r.Name, repoSoxCompliantDemo)
                .With(r => r.Id, guid.ToString()));
        }

        private static void CustomizeScope(Fixture fixture, Guid guid = default(Guid),
            string refName = "refs/heads/master")
        {
            fixture.Customize<VstsService.Response.Scope>(ctx => ctx
                .With(r => r.RepositoryId, guid)
                .With(r => r.RefName, refName));
        }

        private static void CustomizePolicySettings(Fixture fixture, int minimumApproverCount = 2,
            bool resetOnSourcePush = true, bool creatorVoteCounts = true)
        {
            fixture.Customize<VstsService.Response.MinimumNumberOfReviewersPolicySettings>(ctx => ctx
                .With(r => r.MinimumApproverCount, minimumApproverCount)
                .With(r => r.ResetOnSourcePush, resetOnSourcePush)
                .With(r => r.CreatorVoteCounts, creatorVoteCounts));
        }

        private static void CustomizeMinimumNumberOfReviewersPolicy(Fixture fixture, bool enabled = true)
        {
            fixture.Customize<VstsService.Response.MinimumNumberOfReviewersPolicy>(ctx => ctx
                .With(r => r.IsEnabled, enabled));
        }


        private static void InitializeData(IVstsRestClient client, Fixture fixture)
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
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
        private string repoSoxCompliantDemo = "3167b64e-c72b-4c55-84eb-986ac62d0dec";

        public MasterReleaseBranchesProtectedWith4EyesTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        public void EvaluateShouldReturnFalseForRepoNotMatchingPolicies()
        {
            var fixture = new Fixture {RepeatCount = 1};
            fixture.Customize(new AutoNSubstituteCustomization());

            fixture.Customize<VstsService.Response.Repository>(ctx => ctx
                .With(r => r.Name, repoSoxCompliantDemo));

            var client = Substitute.For<IVstsRestClient>();

            var rule = new MasterAndReleaseBranchesProtected(client);

            client
                .Get(Arg.Any<IVstsRestRequest<Multiple<Repository>>>())
                .Returns(fixture.Create<Multiple<Repository>>());

            client
                .Get(Arg.Any<IVstsRestRequest<Multiple<MinimumNumberOfReviewersPolicy>>>())
                .Returns(fixture.Create<Multiple<MinimumNumberOfReviewersPolicy>>());

            rule
                .Evaluate(_config.Project, repoSoxCompliantDemo)
                .ShouldBeFalse();
        }

        [Fact]
        public void EvaluateShouldReturnTrueForRepoHavingCorrectPolicies()
        {
            var fixture = new Fixture {RepeatCount = 1};
            fixture.Customize(new AutoNSubstituteCustomization());

            var id = Guid.NewGuid();

            fixture.Customize<VstsService.Response.Repository>(ctx => ctx
                .With(r => r.Name, repoSoxCompliantDemo)
                .With(r => r.Id, id.ToString()));

            fixture.Customize<VstsService.Response.Scope>(ctx => ctx
                .With(r => r.RepositoryId, id)
                .With(r => r.RefName, "refs/heads/master"));

            fixture.Customize<VstsService.Response.MinimumNumberOfReviewersPolicy>(ctx => ctx
                .With(r => r.IsEnabled, true));

            fixture.Customize<VstsService.Response.MinimumNumberOfReviewersPolicySettings>(ctx => ctx
                .With(r => r.MinimumApproverCount, 2)
                .With(r => r.ResetOnSourcePush, true)
                .With(r => r.CreatorVoteCounts, true));


            var client = Substitute.For<IVstsRestClient>();

            var rule = new MasterAndReleaseBranchesProtected(client);

            client
                .Get(Arg.Any<IVstsRestRequest<Multiple<Repository>>>())
                .Returns(fixture.Create<Multiple<Repository>>());

            client
                .Get(Arg.Any<IVstsRestRequest<Multiple<MinimumNumberOfReviewersPolicy>>>())
                .Returns(fixture.Create<Multiple<MinimumNumberOfReviewersPolicy>>());

            rule.Evaluate(_config.Project, repoSoxCompliantDemo).ShouldBeTrue();
        }

        [Fact]
        public void EvaluateShouldReturnFalseWhenMinimumApproverCountIsLessThan2()
        {
            var fixture = new Fixture {RepeatCount = 1};
            fixture.Customize(new AutoNSubstituteCustomization());

            var id = Guid.NewGuid();

            fixture.Customize<VstsService.Response.Repository>(ctx => ctx
                .With(r => r.Name, repoSoxCompliantDemo)
                .With(r => r.Id, id.ToString()));

            fixture.Customize<VstsService.Response.Scope>(ctx => ctx
                .With(r => r.RepositoryId, id)
                .With(r => r.RefName, "refs/heads/master"));


            fixture.Customize<VstsService.Response.MinimumNumberOfReviewersPolicy>(ctx => ctx
                .With(r => r.IsEnabled, true));

            fixture.Customize<VstsService.Response.MinimumNumberOfReviewersPolicySettings>(ctx => ctx
                .With(r => r.MinimumApproverCount, 1)
                .With(r => r.ResetOnSourcePush, true)
                .With(r => r.CreatorVoteCounts, true));


            var client = Substitute.For<IVstsRestClient>();

            var rule = new MasterAndReleaseBranchesProtected(client);

            client
                .Get(Arg.Any<IVstsRestRequest<Multiple<Repository>>>())
                .Returns(fixture.Create<Multiple<Repository>>());

            client
                .Get(Arg.Any<IVstsRestRequest<Multiple<MinimumNumberOfReviewersPolicy>>>())
                .Returns(fixture.Create<Multiple<MinimumNumberOfReviewersPolicy>>());

            rule.Evaluate(_config.Project, repoSoxCompliantDemo).ShouldBeFalse();
        }

        [Fact]
        public void EvaluateShouldReturnFalseWhenThereAreNoCorrectPoliciesForMasterBranch()
        {
            var fixture = new Fixture {RepeatCount = 1};
            fixture.Customize(new AutoNSubstituteCustomization());

            var id = Guid.NewGuid();

            fixture.Customize<VstsService.Response.Repository>(ctx => ctx
                .With(r => r.Name, repoSoxCompliantDemo)
                .With(r => r.Id, id.ToString()));

            fixture.Customize<VstsService.Response.Scope>(ctx => ctx
                .With(r => r.RepositoryId, id));

            fixture.Customize<VstsService.Response.MinimumNumberOfReviewersPolicy>(ctx => ctx
                .With(r => r.IsEnabled, true));

            fixture.Customize<VstsService.Response.MinimumNumberOfReviewersPolicySettings>(ctx => ctx
                .With(r => r.MinimumApproverCount, 2)
                .With(r => r.ResetOnSourcePush, true)
                .With(r => r.CreatorVoteCounts, true));


            var client = Substitute.For<IVstsRestClient>();

            var rule = new MasterAndReleaseBranchesProtected(client);

            client
                .Get(Arg.Any<IVstsRestRequest<Multiple<Repository>>>())
                .Returns(fixture.Create<Multiple<Repository>>());

            client
                .Get(Arg.Any<IVstsRestRequest<Multiple<MinimumNumberOfReviewersPolicy>>>())
                .Returns(fixture.Create<Multiple<MinimumNumberOfReviewersPolicy>>());

            rule.Evaluate(_config.Project, repoSoxCompliantDemo).ShouldBeFalse();
        }
    }
}
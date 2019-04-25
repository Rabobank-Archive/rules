//using AutoFixture;
//using SecurePipelineScan.Rules.Checks;
//using Shouldly;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Xunit;
//using MinimumNumberOfReviewersPolicy = SecurePipelineScan.VstsService.Response.MinimumNumberOfReviewersPolicy;
//
//namespace SecurePipelineScan.Rules.Tests.Checks
//{
//    public class RepositoryTests
//    {
//        [Fact]
//        public void HasRequiredReviewerPolicy_WhenNoPolicies_ShouldBeFalse()
//        {
//            var fixture = new Fixture();
//            var repoSubstitute = fixture.Create<VstsService.Response.Repository>();
//
//            Repository.HasRequiredReviewerPolicy(repoSubstitute,
//                Enumerable.Empty<MinimumNumberOfReviewersPolicy>()).
//                ShouldBeFalse();
//        }
//
//        [Fact]
//        public void HasRequiredReviewerPolicy_With1Reviewer_ShouldReturnFalse()
//        {
//            Guid projectId = Guid.NewGuid();
//            var fixture = new Fixture();
//
//            var repoSubstitute = fixture.Create<VstsService.Response.Repository>();
//            repoSubstitute.Id = projectId.ToString();
//
//            var policy = CreateValidMinimumNumberOfReviewersPolicy(projectId);
//            policy.Settings.MinimumApproverCount = 1;
//
//            var list = new List<MinimumNumberOfReviewersPolicy>()
//            {
//                 policy
//            };
//
//            Repository.HasRequiredReviewerPolicy(repoSubstitute, list).
//                ShouldBeFalse();
//        }
//
//        [Fact]
//        public void HasRequiredReviewerPolicy_ResetOnSourcePushDisabled_ShouldReturnFalse()
//        {
//            Guid projectId = Guid.NewGuid();
//            var fixture = new Fixture();
//
//            var repoSubstitute = fixture.Create<VstsService.Response.Repository>();
//            repoSubstitute.Id = projectId.ToString();
//
//            var policy = CreateValidMinimumNumberOfReviewersPolicy(projectId);
//            policy.Settings.ResetOnSourcePush = false;
//
//            var list = new List<MinimumNumberOfReviewersPolicy>()
//            {
//                 policy
//            };
//
//            Repository.HasRequiredReviewerPolicy(repoSubstitute, list).
//                ShouldBeFalse();
//        }
//
//        /// <summary>
//        /// This checks if CreatorVote Counts is disabled.
//        /// </summary>
//        [Fact]
//        public void HasRequiredReviewerPolicy_AllowUsersToApproveDisabled_ShouldReturnFalse()
//        {
//            Guid projectId = Guid.NewGuid();
//            var fixture = new Fixture();
//
//            var repoSubstitute = fixture.Create<VstsService.Response.Repository>();
//            repoSubstitute.Id = projectId.ToString();
//
//            var policy = CreateValidMinimumNumberOfReviewersPolicy(projectId);
//            policy.Settings.CreatorVoteCounts = false;
//
//            var list = new List<MinimumNumberOfReviewersPolicy>()
//            {
//                 policy
//            };
//
//            Repository.HasRequiredReviewerPolicy(repoSubstitute, list).
//                ShouldBeFalse();
//        }
//
//        [Fact]
//        public void HasRequiredReviewerPolicy_ShouldReturnTrue()
//        {
//            Guid projectId = Guid.NewGuid();
//            var fixture = new Fixture();
//
//            var repoSubstitute = fixture.Create<VstsService.Response.Repository>();
//            repoSubstitute.Id = projectId.ToString();
//
//            var policy = CreateValidMinimumNumberOfReviewersPolicy(projectId);
//
//            var list = new List<MinimumNumberOfReviewersPolicy>()
//            {
//                 policy
//            };
//
//            Repository.HasRequiredReviewerPolicy(repoSubstitute, list).
//                ShouldBeTrue();
//        }
//
//        /// <summary>
//        /// Creates a minimumnumber of reviewerspolicy which returns true on validation
//        /// </summary>
//        /// <param name="repId"></param>
//        /// <returns></returns>
//        private static MinimumNumberOfReviewersPolicy CreateValidMinimumNumberOfReviewersPolicy(Guid repId)
//        {
//            return new MinimumNumberOfReviewersPolicy
//            {
//                IsEnabled = true,
//                IsBlocking = true,
//                Settings = new VstsService.Response.MinimumNumberOfReviewersPolicySettings
//                {
//                    ResetOnSourcePush = true,
//                    AllowDownvotes = false,
//                    CreatorVoteCounts = true,
//                    MinimumApproverCount = 2,
//                    Scope = new List<VstsService.Response.Scope>() {
//                        new VstsService.Response.Scope
//                        {
//                            RefName = "refs/heads/master",
//                            MatchKind = "Exact",
//                            RepositoryId = repId,
//                        }
//                    }
//                }
//            };
//        }
//    }
//}
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Tests;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using RestSharp;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class Policies: IClassFixture<TestConfig>
    {
        private readonly TestConfig config;
        private readonly IVstsRestClient Vsts;

        public Policies(TestConfig config)
        {
            this.config = config;
            Vsts = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryRequiredReviewersPolicies()
        {
            var definition = Vsts.Get(Requests.Policies.RequiredReviewersPolicies(config.Project));

            definition.ShouldNotBeEmpty();
            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.Id));
            definition.ShouldAllBe(e => e.IsBlocking.HasValue);
            definition.ShouldAllBe(e => e.IsDeleted.HasValue);
            definition.ShouldAllBe(e => e.IsEnabled.HasValue);
            definition.ShouldAllBe(e => e.Settings.RequiredReviewerIds.Count > 0);
            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].MatchKind));
            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].RefName));
            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].RepositoryId.ToString()));
        }

        [Fact]
        public void QueryMinimumNumberOfReviewersPolicies()
        {
            var definition = Vsts.Get(Requests.Policies.MinimumNumberOfReviewersPolicies(config.Project));

            definition.ShouldNotBeEmpty();
            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.Id));
            definition.ShouldAllBe(e => e.IsBlocking.HasValue);
            definition.ShouldAllBe(e => e.IsDeleted.HasValue);
            definition.ShouldAllBe(e => e.IsEnabled.HasValue);

            definition.ShouldAllBe(e => e.Settings.MinimumApproverCount.HasValue);
            definition.ShouldAllBe(e => e.Settings.AllowDownvotes.HasValue);
            definition.ShouldAllBe(e => e.Settings.CreatorVoteCounts.HasValue);
            definition.ShouldAllBe(e => e.Settings.ResetOnSourcePush.HasValue);

            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].MatchKind));
            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].RefName));
            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].RepositoryId.ToString()));
        }
    }
}

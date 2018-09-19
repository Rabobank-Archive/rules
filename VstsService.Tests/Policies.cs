using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Tests;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
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
            var definition = Vsts.Execute(Requests.Policies.RequiredReviewersPolicies(config.Project));

            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Data.Value.ShouldNotBeEmpty();
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Id));
            definition.Data.Value.ShouldAllBe(e => e.IsBlocking.HasValue);
            definition.Data.Value.ShouldAllBe(e => e.IsDeleted.HasValue);
            definition.Data.Value.ShouldAllBe(e => e.IsEnabled.HasValue);
            definition.Data.Value.ShouldAllBe(e => e.Settings.RequiredReviewerIds.Count > 0);
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].MatchKind));
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].RefName));
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].RepositoryId.ToString()));
        }

        [Fact]
        public void QueryMinimumNumberOfReviewersPolicies()
        {
            var definition = Vsts.Execute(Requests.Policies.MinimumNumberOfReviewersPolicies(config.Project));

            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Data.Value.ShouldNotBeEmpty();
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Id));
            definition.Data.Value.ShouldAllBe(e => e.IsBlocking.HasValue);
            definition.Data.Value.ShouldAllBe(e => e.IsDeleted.HasValue);
            definition.Data.Value.ShouldAllBe(e => e.IsEnabled.HasValue);

            definition.Data.Value.ShouldAllBe(e => e.Settings.MinimumApproverCount.HasValue);
            definition.Data.Value.ShouldAllBe(e => e.Settings.AllowDownvotes.HasValue);
            definition.Data.Value.ShouldAllBe(e => e.Settings.CreatorVoteCounts.HasValue);
            definition.Data.Value.ShouldAllBe(e => e.Settings.ResetOnSourcePush.HasValue);

            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].MatchKind));
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].RefName));
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].RepositoryId.ToString()));
        }
    }
}

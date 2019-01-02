using Newtonsoft.Json;
using SecurePipelineScan.VstsService.Converters;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class Policies : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _vsts;

        public Policies(TestConfig config)
        {
            this._config = config;
            _vsts = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryRequiredReviewersPolicies()
        {
            var result = _vsts.Get(Requests.Policies.RequiredReviewersPolicies(_config.Project));

            result.ShouldNotBeEmpty();
            result.ShouldAllBe(e => !string.IsNullOrEmpty(e.Id));
            result.ShouldAllBe(e => e.IsBlocking.HasValue);
            result.ShouldAllBe(e => e.IsDeleted.HasValue);
            result.ShouldAllBe(e => e.IsEnabled.HasValue);
            result.ShouldAllBe(e => e.Settings.RequiredReviewerIds.Count > 0);
            result.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].MatchKind));
            result.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].RefName));
            result.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].RepositoryId.ToString()));
        }

        [Fact]
        public void QueryMinimumNumberOfReviewersPolicies()
        {
            var result = _vsts.Get(Requests.Policies.MinimumNumberOfReviewersPolicies(_config.Project));

            result.ShouldNotBeEmpty();
            result.ShouldAllBe(e => !string.IsNullOrEmpty(e.Id));
            result.ShouldAllBe(e => e.IsBlocking.HasValue);
            result.ShouldAllBe(e => e.IsDeleted.HasValue);
            result.ShouldAllBe(e => e.IsEnabled.HasValue);

            result.ShouldAllBe(e => e.Settings.MinimumApproverCount.HasValue);
            result.ShouldAllBe(e => e.Settings.AllowDownvotes.HasValue);
            result.ShouldAllBe(e => e.Settings.CreatorVoteCounts.HasValue);
            result.ShouldAllBe(e => e.Settings.ResetOnSourcePush.HasValue);

            result.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].MatchKind));
            result.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].RefName));
            result.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].RepositoryId.ToString()));
        }

        [Fact]
        public void GetAllPoliciesForProject()
        {
            var result = _vsts.Get(Requests.Policies.All(_config.Project));

            result.ShouldNotBeEmpty();
            result.ShouldAllBe(e => !string.IsNullOrEmpty(e.Id));
            result.ShouldAllBe(e => e.IsBlocking.HasValue);
            result.ShouldAllBe(e => e.IsDeleted.HasValue);
            result.ShouldAllBe(e => e.IsEnabled.HasValue);
        }

        [Fact]
        public void GetAllPoliciesConvertsToSpecific()
        {
            var policies = _vsts.Get(Requests.Policies.All(_config.Project));

            policies.ShouldContain(p => p is RequiredReviewersPolicy);
            policies.ShouldContain(p => p is MinimumNumberOfReviewersPolicy);
        }
    }
}
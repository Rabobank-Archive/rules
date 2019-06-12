using System;
using Newtonsoft.Json;
using SecurePipelineScan.VstsService.Converters;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class Policies : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;

        public Policies(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public async Task QueryRequiredReviewersPolicies()
        {
            var result = await _client.GetAsync(Requests.Policies.RequiredReviewersPolicies(_config.Project));

            var requiredReviewersPolicies = result.ToList();
            requiredReviewersPolicies.ShouldNotBeEmpty();
            requiredReviewersPolicies.Any(e => e.Id != 0).ShouldBeTrue();
            requiredReviewersPolicies.Any(e => e.IsBlocking).ShouldBeTrue();
            requiredReviewersPolicies.All(e => e.IsDeleted).ShouldBeFalse();
            requiredReviewersPolicies.All(e => e.IsEnabled).ShouldBeTrue();
            requiredReviewersPolicies.ShouldAllBe(e => e.Settings.RequiredReviewerIds.Count > 0);
            requiredReviewersPolicies.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].MatchKind));
            requiredReviewersPolicies.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].RefName));
            requiredReviewersPolicies.ShouldAllBe(e => e.Settings.Scope[0].RepositoryId != Guid.Empty);
        }

        [Fact]
        public async Task QueryMinimumNumberOfReviewersPolicies()
        {
            var result = await _client.GetAsync(Requests.Policies.MinimumNumberOfReviewersPolicies(_config.Project));

            var minimumNumberOfReviewersPolicies = result.ToList();
            minimumNumberOfReviewersPolicies.ShouldNotBeEmpty();
            minimumNumberOfReviewersPolicies.Any(e => e.Id != 0).ShouldBeTrue();
            minimumNumberOfReviewersPolicies.Any(e => e.IsBlocking).ShouldBeTrue();
            minimumNumberOfReviewersPolicies.All(e => e.IsDeleted).ShouldBeFalse();
            minimumNumberOfReviewersPolicies.Any(e => e.IsEnabled).ShouldBeTrue();

            minimumNumberOfReviewersPolicies.Any(e => e.Settings.MinimumApproverCount != 0).ShouldBeTrue();
            minimumNumberOfReviewersPolicies.All(e => e.Settings.AllowDownvotes).ShouldBeFalse();
            minimumNumberOfReviewersPolicies.Any(e => e.Settings.CreatorVoteCounts).ShouldBeTrue();
            minimumNumberOfReviewersPolicies.Any(e => e.Settings.ResetOnSourcePush).ShouldBeTrue();

            minimumNumberOfReviewersPolicies.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].MatchKind));
            minimumNumberOfReviewersPolicies.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].RefName));
            minimumNumberOfReviewersPolicies.ShouldAllBe(e => e.Settings.Scope[0].RepositoryId != Guid.Empty);
        }

        [Fact]
        public async Task QueryPolicy()
        {
            var result = await _client.GetAsync(Requests.Policies.Policy(_config.Project, 28));
            result.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetAllPoliciesForProject()
        {
            var result = await _client.GetAsync(Requests.Policies.All(_config.Project));

            var enumerable = result.ToList();
            enumerable.ShouldNotBeEmpty();
            enumerable.ShouldAllBe(e => e.Id != 0);
            enumerable.Any(e => e.IsBlocking).ShouldBeTrue();
            enumerable.All(e => e.IsDeleted).ShouldBeFalse();
            enumerable.Any(e => e.IsEnabled).ShouldBeTrue();
        }

        [Fact]
        public async Task GetAllPoliciesConvertsToSpecific()
        {
            var policies = await _client.GetAsync(Requests.Policies.All(_config.Project));

            var policiesToList = policies as Policy[] ?? policies.ToArray();
            policiesToList.ShouldContain(p => p is RequiredReviewersPolicy);
            policiesToList.ShouldContain(p => p is MinimumNumberOfReviewersPolicy);
        }
    }
}
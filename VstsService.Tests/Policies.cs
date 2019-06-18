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
        public void QueryRequiredReviewersPolicies()
        {
            var result = _client.Get(Requests.Policies.RequiredReviewersPolicies(_config.Project)).ToList();

            result.ShouldNotBeEmpty();
            result.Any(e => e.Id != 0).ShouldBeTrue();
            result.Any(e => e.IsBlocking).ShouldBeTrue();
            result.All(e => e.IsDeleted).ShouldBeFalse();
            result.All(e => e.IsEnabled).ShouldBeTrue();
            result.ShouldAllBe(e => e.Settings.RequiredReviewerIds.Count > 0);
            result.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].MatchKind));
            result.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].RefName));
            result.ShouldAllBe(e => e.Settings.Scope[0].RepositoryId != Guid.Empty);
        }

        [Fact]
        public void QueryMinimumNumberOfReviewersPolicies()
        {
            var result = _client.Get(Requests.Policies.MinimumNumberOfReviewersPolicies(_config.Project)).ToList();

            result.ShouldNotBeEmpty();
            result.Any(e => e.Id != 0).ShouldBeTrue();
            result.Any(e => e.IsBlocking).ShouldBeTrue();
            result.All(e => e.IsDeleted).ShouldBeFalse();
            result.Any(e => e.IsEnabled).ShouldBeTrue();

            result.Any(e => e.Settings.MinimumApproverCount != 0).ShouldBeTrue();
            result.All(e => e.Settings.AllowDownvotes).ShouldBeFalse();
            result.Any(e => e.Settings.CreatorVoteCounts).ShouldBeTrue();
            result.Any(e => e.Settings.ResetOnSourcePush).ShouldBeTrue();

            result.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].MatchKind));
            result.ShouldAllBe(e => !string.IsNullOrEmpty(e.Settings.Scope[0].RefName));
            result.ShouldAllBe(e => e.Settings.Scope[0].RepositoryId != Guid.Empty);
        }

        [Fact]
        public async Task QueryPolicy()
        {
            var result = await _client.GetAsync(Requests.Policies.Policy(_config.Project, 28));
            result.ShouldNotBeNull();
        }

        [Fact]
        public void GetAllPoliciesForProject()
        {
            var result = _client.Get(Requests.Policies.All(_config.Project)).ToList();

            result.ShouldNotBeEmpty();
            result.ShouldAllBe(e => e.Id != 0);
            result.Any(e => e.IsBlocking).ShouldBeTrue();
            result.All(e => e.IsDeleted).ShouldBeFalse();
            result.Any(e => e.IsEnabled).ShouldBeTrue();
        }

        [Fact]
        public void GetAllPoliciesConvertsToSpecific()
        {
            var policies = _client.Get(Requests.Policies.All(_config.Project)).ToList();

            policies.ShouldContain(p => p is RequiredReviewersPolicy);
            policies.ShouldContain(p => p is MinimumNumberOfReviewersPolicy);
        }
    }
}
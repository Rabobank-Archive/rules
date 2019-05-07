using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Tests;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class ApplicationGroup : IClassFixture<TestConfig>
    {
        private readonly TestConfig config;
        private readonly IVstsRestClient Vsts;

        public ApplicationGroup(TestConfig config)
        {
            this.config = config;
            Vsts = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryTASApplicationGroupDataReturnsGroupData()
        {
            var identity = Vsts.Get(Requests.ApplicationGroup.ApplicationGroups(config.Project));
            identity.ShouldNotBeNull();
            identity.Identities.ShouldNotBeEmpty();

            var group = identity.Identities.First();
            group.DisplayName.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void ExplicitIdentitiesShouldGetIdentities()
        {
            string projectId = "53410703-e2e5-4238-9025-233bd7c811b3";
            string nameSpaceId = "2e9eb7ed-3c0a-47d4-87c1-0ffdd275fd87";

            var explicitIdentities = Vsts.Get(Requests.ApplicationGroup.ExplicitIdentitiesRepos(projectId, nameSpaceId));
            explicitIdentities.ShouldNotBeNull();
        }

        [Fact]
        public void ExplicitIdentitiesForBuildDefinitionShouldGetIdentities()
        {
            string projectId = "53410703-e2e5-4238-9025-233bd7c811b3";
            string nameSpaceId = "33344d9c-fc72-4d6f-aba5-fa317101a7e9";
            string buildPipelineId = "2";

            var explicitIdentities = Vsts.Get(Requests.ApplicationGroup.ExplicitIdentitiesPipelines(projectId, nameSpaceId, buildPipelineId));
            explicitIdentities.ShouldNotBeNull();
            explicitIdentities.Identities.ShouldNotBeEmpty();
        }
    }
}
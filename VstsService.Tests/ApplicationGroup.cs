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
            var identity = Vsts.Execute(Requests.ApplicationGroup.ApplicationGroups(config.Project));
            identity.StatusCode.ShouldBe(HttpStatusCode.OK);
            identity.Data.ShouldNotBeNull();
            identity.Data.Identities.ShouldNotBeEmpty();

            var group = identity.Data.Identities.First();
            group.DisplayName.ShouldNotBeNullOrEmpty();
        }
    }
}
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
            var definition = Vsts.Execute(Requests.ApplicationGroup.ApplicationGroups(config.Project));
            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Content.ShouldNotBeEmpty();
            definition.Content.Contains("Production Environment Owners");
        }
    }
}
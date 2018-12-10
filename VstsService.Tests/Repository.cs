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
    public class Repository: IClassFixture<TestConfig>
    {
        private readonly TestConfig config;
        private readonly IVstsRestClient Vsts;

        public Repository(TestConfig config)
        {
            this.config = config;
            Vsts = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryRepository()
        {
            var definition = Vsts.Get(Requests.Repository.Repositories(config.Project));
            definition.Value.ShouldNotBeEmpty();
            definition.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Name));
            definition.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Id));
            definition.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Project.Id));
            definition.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Project.Name));
            definition.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.DefaultBranch));
        }
    }
}

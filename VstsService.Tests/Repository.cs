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
            var definition = Vsts.Execute(Requests.Repository.Repositories(config.Project));

            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Data.Value.ShouldNotBeEmpty();
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Name));
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Id));
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Project.Id));
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Project.Name));
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.DefaultBranch));
        }
    }
}

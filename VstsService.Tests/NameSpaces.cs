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
    [Trait("Category","integration")]
    public class NameSpaces : IClassFixture<TestConfig>
    {
        private readonly TestConfig config;
        private readonly IVstsRestClient Vsts;

        public NameSpaces(TestConfig config)
        {
            this.config = config;
            Vsts = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryNameSpaces()
        {
            var nameSpaces = Vsts.Get(Requests.NameSpace.NameSpaces()).Value;
            
            nameSpaces.ShouldNotBeEmpty();
        }
    }
}
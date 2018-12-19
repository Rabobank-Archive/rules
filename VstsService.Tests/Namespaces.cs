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
    public class Namespaces : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;

        public Namespaces(TestConfig config)
        {
            this._config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryNameSpaces()
        {
            var nameSpaces = _client.Get(Requests.Namespace.SecurityNamespaces()).Value;
            
            nameSpaces.ShouldNotBeEmpty();
        }
    }
}
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
    public class SecurityNamespaces : IClassFixture<TestConfig>
    {
        private readonly IVstsRestClient _client;

        public SecurityNamespaces(TestConfig config)
        {
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryNamespaces()
        {
            var target = _client.Get(Requests.SecurityNamespace.SecurityNamespaces());
            target.ShouldNotBeEmpty();

            var first = target.First();
            first.Actions.ShouldNotBeEmpty();

            var action = first.Actions.First();
            action.Name.ShouldNotBeEmpty();
            action.DisplayName.ShouldNotBeEmpty();
            action.NamespaceId.ShouldNotBe(Guid.Empty);
            action.Bit.ShouldNotBe(0);
        }
    }
}
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Tests;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using SecurePipelineScan.VstsService.Response;
using Xunit;
using Task = System.Threading.Tasks.Task;

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
        public async Task QueryNamespaces()
        {
            var target = (await _client.GetAsync(Requests.SecurityNamespace.SecurityNamespaces())).ToList();
            target.ShouldNotBeEmpty();

            var first = target.First();
            first.Actions.ShouldNotBeEmpty();

            var action = first.Actions.First();
            action.Name.ShouldNotBeEmpty();
            action.DisplayName.ShouldNotBeEmpty();
            action.Bit.ShouldNotBe(0);
        }
    }
}
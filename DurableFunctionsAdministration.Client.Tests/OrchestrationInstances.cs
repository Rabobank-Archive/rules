using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DurableFunctionsAdministration.Client.Tests
{
    [Trait("category", "integration")]
    public class OrchestrationInstances : IClassFixture<TestConfig>
    {
        private readonly IDurableFunctionsAdministrationClient _client;

        public OrchestrationInstances(TestConfig config)
        {
            _client = new DurableFunctionsAdministrationClient(new Uri(config.BaseUri), config.TaskHub, config.Code);
        }

        [Fact]
        public async Task GetInstancesReturnsInstances()
        {
            var instances = await _client.GetAsync(Request.OrchestrationInstances.List());
            instances.ShouldNotBeNull();
        }
    }
}
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DurableFunctionsAdministration.Client.Tests
{
    [Trait("category", "integration")]
    public class OrchestrationInstances : IClassFixture<TestConfig>
    {
        private readonly TestConfig config;
        private readonly IDurableFunctionsAdministrationClient Client;

        public OrchestrationInstances(TestConfig config)
        {
            this.config = config;
            Client = new DurableFunctionsAdministrationClient(new Uri(config.BaseUri), config.TaskHub, config.Code);
        }

        [Fact]
        public async Task GetInstancesReturnsInstances()
        {
            var instances = await Client.GetAsync(Request.OrchestrationInstances.List());
            instances.ShouldNotBeNull();
        }
    }
}
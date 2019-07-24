using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DurableFunctionsAdministration.Client.Response;
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
        public void ListInstancesReturnsInstances()
        {
            var instances = _client.Get(Request.OrchestrationInstances.List());
            var orchestrationInstances = instances as OrchestrationInstance[] ?? instances.ToArray();
            
            orchestrationInstances.ShouldNotBeNull();
            var first = orchestrationInstances.First();
            first.CreatedTime.ShouldNotBeNull();
            first.InstanceId.ShouldNotBeNull();
            first.RuntimeStatus.ShouldNotBeNull();
            first.LastUpdatedTime.ShouldNotBeNull();
        }
        
        [Fact]
        public async Task GetInstanceReturnsInstance()
        {
            var instances = _client.Get(Request.OrchestrationInstances.List());
            var instanceId = instances.First().InstanceId;
  
            var instance = await _client.GetAsync(Request.OrchestrationInstances.Get(instanceId));

//            var instance = await _client.GetAsync(Request.OrchestrationInstances.Get("00ae37f0d17c46afbbcf9098661d7371:0"));
            
            instance.ShouldNotBeNull();
            instance.CreatedTime.ShouldNotBeNull();
            instance.InstanceId.ShouldNotBeNull();
            instance.RuntimeStatus.ShouldNotBeNull();
            instance.LastUpdatedTime.ShouldNotBeNull();
        }
    }
}
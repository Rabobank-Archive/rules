using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using DurableFunctionsAdministration.Client.Model;
using DurableFunctionsAdministration.Client.Response;
using Xunit;

namespace DurableFunctionsAdministration.Client.Tests
{
    [Trait("category", "integration")]
    public class OrchestrationInstancesTests : IClassFixture<TestConfig>
    {
        private readonly IDurableFunctionsAdministrationClient _client;

        public OrchestrationInstancesTests(TestConfig config)
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
            first.Name.ShouldNotBeNull();
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

            instance.ShouldNotBeNull();
            instance.Name.ShouldNotBeNull();
            instance.CreatedTime.ShouldNotBeNull();
            instance.InstanceId.ShouldNotBeNull();
            instance.RuntimeStatus.ShouldNotBeNull();
            instance.LastUpdatedTime.ShouldNotBeNull();
        }

        [Theory]
        [InlineData(RunTimeStatusses.Completed)]
        [InlineData(RunTimeStatusses.Failed)]
        [InlineData(RunTimeStatusses.Pending)]
        [InlineData(RunTimeStatusses.Running)]
        [InlineData(RunTimeStatusses.Terminated)]
        [InlineData(RunTimeStatusses.ContinuedAsNew)]
        public void GetInstancesByRunTimeStatusShouldIncludeOnlyThatStatus(string status)
        {
            var instances = _client.Get(Request.OrchestrationInstances.ListByStatus(new[] { status }));

            var orchestrationInstances = instances as OrchestrationInstance[] ?? instances.ToArray();
            orchestrationInstances.All(i => i.RuntimeStatus == status).ShouldBeTrue();
        }
        
        [Fact]
        public void GetInstancesByRunTimeStatusesShouldIncludeOnlyThoseStatuses()
        {
            var instances = _client.Get(Request.OrchestrationInstances.ListByStatus(new[] { RunTimeStatusses.Running, RunTimeStatusses.Failed }));

            var orchestrationInstances = instances as OrchestrationInstance[] ?? instances.ToArray();
            orchestrationInstances.All(i => i.RuntimeStatus == RunTimeStatusses.Running || i.RuntimeStatus == RunTimeStatusses.Failed).ShouldBeTrue();
        }
    }
}
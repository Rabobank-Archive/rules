using System;
using AutoFixture;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests
{
    public class ServiceEndpointFiltersTests
    {
        [Fact]
        public void IncludesNormalEndpoint()
        {
            var fixture = new Fixture();
            var id = fixture.Create<Guid>();

            var endpoint = new {id, url = "some-endpoint.somecompany.nl"};
            var endpoints = JObject.FromObject(new { value = new[] { endpoint } });
            endpoints
                .ProductionEndpoints()
                .ShouldContain(e => (Guid)e["id"] == id);
        }
                
        [Fact]
        public void ExcludesTestUrl()
        {
            var fixture = new Fixture();
            var id = fixture.Create<Guid>();

            var endpoint = new { id, url = "test.somecompany.nl" };
            var endpoints = JObject.FromObject(new { value = new[] { endpoint } });           
            endpoints
                .ProductionEndpoints()
                .ShouldBeEmpty();
        }

        [Fact]
        public void IncludesAzureProductionSubscriptions()
        {
            var fixture = new Fixture();
            var id = fixture.Create<Guid>();

            var endpoint = new
            {
                id, 
                url = "https://management.azure.com/",
                type = "azurerm",
                data = new
                {
                    subscriptionId = "a2439340-3e5e-4290-bc80-89065170bc86"
                }
            };

            var endpoints = JObject.FromObject(new { value = new[] { endpoint } });
            endpoints.ProductionEndpoints()
                .ShouldContain(e => (Guid)e["id"] == id);
        }
        
        [Fact]
        public void ExcludesOtherAzureSubscriptions()
        {
            var fixture = new Fixture();
            var id = fixture.Create<Guid>();

            var endpoint = new
            {
                id, 
                url = "https://management.azure.com/",
                type = "azurerm",
                data = new
                {
                    subscriptionId = fixture.Create<Guid>()
                }
            };
            var endpoints = JObject.FromObject(new { value = new[] { endpoint } });
            endpoints
                .ProductionEndpoints()
                .ShouldBeEmpty();
        }

        [Fact]
        public void FilterEmptyResult()
        {
            var endpoints = JObject.FromObject(new {});
            endpoints
                .ProductionEndpoints()
                .ShouldBeEmpty();
        }
    }
}

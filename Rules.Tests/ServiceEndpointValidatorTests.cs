using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using NSubstitute;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests
{
    public class ServiceEndpointValidatorTests
    {
        [Fact]
        public void UsesEndpointsFromClient()
        {
            var fixture = Fixture();
            var client = Substitute.For<IVstsRestClient>();
            client
                .Get(Arg.Any<IVstsRestRequest<JObject>>())
                .Returns(JObject.FromObject(new { }));

            using (var cache = new MemoryCache(new MemoryCacheOptions()))
            {
                var validator = new ServiceEndpointValidator(client, cache);
                validator
                    .IsProduction(fixture.Create<string>(), fixture.Create<Guid>());
            }
            
            client
                .Received()
                .Get(Arg.Any<IVstsRestRequest<JObject>>());
        }

        [Fact]
        public void CachesResultForSameProject()
        {
            var fixture = Fixture();
            var project = fixture.Create<string>();

            var client = Substitute.For<IVstsRestClient>();
            client
                .Get(Arg.Any<IVstsRestRequest<JObject>>())
                .Returns(JObject.FromObject(new { }));

            using (var cache = new MemoryCache(new MemoryCacheOptions()))
            {
                var validator = new ServiceEndpointValidator(client, cache);
                validator.IsProduction(project, fixture.Create<Guid>());
                validator.IsProduction(project, fixture.Create<Guid>());
            }

            client
                .Received(1)
                .Get(Arg.Any<IVstsRestRequest<JObject>>());
        }

        [Fact]
        public void IncludesEndpointValidation()
        {
            var fixture = Fixture();
            var id = fixture.Create<Guid>();

            var endpoint = new { id, url = "some-endpoint.somecompany.nl" };
            var endpoints = JObject.FromObject(new { value = new[] { endpoint } });
        
            var client = Substitute.For<IVstsRestClient>();
            client
                .Get(Arg.Any<IVstsRestRequest<JObject>>())
                .Returns(JObject.FromObject(endpoints));

            using (var cache = new MemoryCache(new MemoryCacheOptions()))
            {
                var validator = new ServiceEndpointValidator(client, cache);
                validator
                    .IsProduction(fixture.Create<string>(), id)
                    .ShouldBeTrue();
                
                validator
                    .IsProduction(fixture.Create<string>(), fixture.Create<Guid>())
                    .ShouldBeFalse();
            }
        }

        private static IFixture Fixture()
        {
            return new Fixture().Customize(new AutoNSubstituteCustomization());
        }
    }
}

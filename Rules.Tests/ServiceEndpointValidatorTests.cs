using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using NSubstitute;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Tests
{
    public sealed class ServiceEndpointValidatorTests : IDisposable
    {
        private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        [Fact]
        public async Task UsesEndpointsFromClient()
        {
            var fixture = Fixture();
            var client = Substitute.For<IVstsRestClient>();
            client
                .GetAsync(Arg.Any<IVstsRequest<JObject>>())
                .Returns(JObject.FromObject(new { }));

            var validator = new ServiceEndpointValidator(client, _cache);
            await validator
                .ScanForProductionEndpointsAsync(fixture.Create<string>(), fixture.Create<Guid>());
        
            await client
                .Received()
                .GetAsync(Arg.Any<IVstsRequest<JObject>>());
        }

        [Fact]
        public async Task CachesResultForSameProject()
        {
            var fixture = Fixture();
            var project = fixture.Create<string>();

            var client = Substitute.For<IVstsRestClient>();
            client
                .GetAsync(Arg.Any<IVstsRequest<JObject>>())
                .Returns(JObject.FromObject(new { }));

            var validator = new ServiceEndpointValidator(client, _cache);
            await validator.ScanForProductionEndpointsAsync(project, fixture.Create<Guid>());
            await validator.ScanForProductionEndpointsAsync(project, fixture.Create<Guid>());

            await client
                .Received(1)
                .GetAsync(Arg.Any<IVstsRequest<JObject>>());
        }

        [Fact]
        public async Task IncludesEndpointValidation()
        {
            var fixture = Fixture();
            var id = fixture.Create<Guid>();

            var endpoint = new { id, url = "some-endpoint.somecompany.nl" };
            var endpoints = JObject.FromObject(new { value = new[] { endpoint } });
        
            var client = Substitute.For<IVstsRestClient>();
            client
                .GetAsync(Arg.Any<IVstsRequest<JObject>>())
                .Returns(JObject.FromObject(endpoints));

            var validator = new ServiceEndpointValidator(client, _cache);
            (await validator
                .ScanForProductionEndpointsAsync(fixture.Create<string>(), id))
                .ShouldBeTrue();
            
            (await validator
                .ScanForProductionEndpointsAsync(fixture.Create<string>(), fixture.Create<Guid>()))
                .ShouldBeFalse();
        }

        private static IFixture Fixture()
        {
            return new Fixture().Customize(new AutoNSubstituteCustomization());
        }

        public void Dispose()
        {
            _cache.Dispose();
        }
    }
}

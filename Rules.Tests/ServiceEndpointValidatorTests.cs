using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using RestSharp;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests
{
    public class ServiceEndpointValidatorTests
    {
        [Fact]
        public void UsesEndpointsFromClient()
        {
            var client = VstsRestClient(Fixture());
            
            var validator = new ServiceEndpointValidator(client, new MemoryCache(new MemoryCacheOptions()));
            validator
                .IsProduction(Fixture().Create<string>(), Fixture().Create<Guid>());
            
            client
                .Received()
                .Get(Arg.Any<IVstsRestRequest<Multiple<ServiceEndpoint>>>());
        }

        [Fact]
        public void CachesResultForSameProject()
        {
            var fixture = Fixture();
            var project = fixture.Create<string>();

            var client = VstsRestClient(fixture);
            var validator = new ServiceEndpointValidator(client, new MemoryCache(new MemoryCacheOptions()));
                       
            validator.IsProduction(project, fixture.Create<Guid>());
            validator.IsProduction(project, fixture.Create<Guid>());
            
            client
                .Received(1)
                .Get(Arg.Any<IVstsRestRequest<Multiple<ServiceEndpoint>>>());
        }

        [Fact]
        public void TrueForEndpoint()
        {
            var fixture = Fixture();
            fixture.RepeatCount = 1;

            var id = fixture.Create<Guid>();
            fixture.Customize<ServiceEndpoint>(e => e.With(x => x.Url, "some-endpoint.somecompany.nl").With(x => x.Id, id));

            var validator =
                new ServiceEndpointValidator(VstsRestClient(fixture), new MemoryCache(new MemoryCacheOptions()));

            validator
                .IsProduction(fixture.Create<string>(), id)
                .ShouldBeTrue();
        }
                
        [Fact]
        public void FalseForUnknownEndpoint()
        {
            var client = VstsRestClient(Fixture());
            
            var validator = new ServiceEndpointValidator(client, new MemoryCache(new MemoryCacheOptions()));
            validator
                .IsProduction(Fixture().Create<string>(), Fixture().Create<Guid>())
                .ShouldBeFalse();
        }

        [Fact]
        public void FalseForEndpointContainingTestInUrl()
        {
            var fixture = Fixture();
            fixture.RepeatCount = 1;

            var id = fixture.Create<Guid>();
            fixture.Customize<ServiceEndpoint>(e => e.With(x => x.Url, "test.somecompany.nl").With(x => x.Id, id));

            var validator = new ServiceEndpointValidator(VstsRestClient(fixture), new MemoryCache(new MemoryCacheOptions()));

            validator
                .IsProduction(fixture.Create<string>(), id)
                .ShouldBeFalse();
                
        }

        private static IVstsRestClient VstsRestClient(IFixture fixture)
        {
            var client = Substitute.For<IVstsRestClient>();
            client
                .Get(Arg.Any<IVstsRestRequest<Multiple<ServiceEndpoint>>>())
                .Returns(fixture.Create<Multiple<ServiceEndpoint>>());
            
            return client;
        }

        private static IFixture Fixture()
        {
            return new Fixture().Customize(new AutoNSubstituteCustomization());
        }
    }
}

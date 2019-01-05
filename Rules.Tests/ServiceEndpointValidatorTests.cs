using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using NSubstitute;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests
{
    public class ServiceEndpointValidatorTests : IDisposable
    {
        private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        [Fact]
        public void UsesEndpointsFromClient()
        {
            var fixture = Fixture();
            var client = Substitute.For<IVstsRestClient>();
            client
                .Get(Arg.Any<IVstsRestRequest<JObject>>())
                .Returns(JObject.FromObject(new { }));

            var validator = new ServiceEndpointValidator(client, _cache);
            validator
                .IsProduction(fixture.Create<string>(), fixture.Create<Guid>());
        
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

            var validator = new ServiceEndpointValidator(client, _cache);
            validator.IsProduction(project, fixture.Create<Guid>());
            validator.IsProduction(project, fixture.Create<Guid>());

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

            var validator = new ServiceEndpointValidator(client, _cache);
            validator
                .IsProduction(fixture.Create<string>(), id)
                .ShouldBeTrue();
            
            validator
                .IsProduction(fixture.Create<string>(), fixture.Create<Guid>())
                .ShouldBeFalse();
        }

        [Fact]
        public void CheckReleaseEnvironment()
        {
            var fixture = Fixture();
            var endpointId = fixture.Create<Guid>();

            fixture.Register(() => new WorkflowTask
            {
                Inputs = new Dictionary<string, string>()
                {
                    { "some-endpoint-name", endpointId.ToString() }
                }
            });

            var endpoint = new { id = endpointId, url = "some-endpoint.somecompany.nl" };
            var endpoints = JObject.FromObject(new { value = new[] { endpoint } });
        
            var client = Substitute.For<IVstsRestClient>();
            client
                .Get(Arg.Any<IVstsRestRequest<VstsService.Response.Environment>>())
                .Returns(fixture.Create<VstsService.Response.Environment>());

            client
                .Get(Arg.Any<IVstsRestRequest<JObject>>())
                .Returns(JObject.FromObject(endpoints));

            var project = fixture.Create<string>();
            var releaseId = fixture.Create<string>();
            var environmentId = fixture.Create<string>();
            
            var validator = new ServiceEndpointValidator(client, _cache);
            validator
                .CheckReleaseEnvironment(project, releaseId, environmentId)
                .ShouldBeTrue();
            
            client
                .Received()
                .Get(Arg.Is<IVstsRestRequest<VstsService.Response.Environment>>(
                    x => x.Uri.Contains(project) && 
                         x.Uri.Contains(releaseId) && 
                         x.Uri.Contains(environmentId)));

            client
                .Received()
                .Get(Arg.Any<IVstsRestRequest<JObject>>());
        }
        
        [Fact]
        public void CheckReleaseEnvironmentUsingNoEndpoint()
        {
            var fixture = Fixture();

            var endpoint = new { id = fixture.Create<Guid>(), url = "some-endpoint.somecompany.nl" };
            var endpoints = JObject.FromObject(new { value = new[] { endpoint } });
        
            var client = Substitute.For<IVstsRestClient>();
            client
                .Get(Arg.Any<IVstsRestRequest<VstsService.Response.Environment>>())
                .Returns(fixture.Create<VstsService.Response.Environment>());

            client
                .Get(Arg.Any<IVstsRestRequest<JObject>>())
                .Returns(JObject.FromObject(endpoints));

            var validator = new ServiceEndpointValidator(client, _cache);
            validator
                .CheckReleaseEnvironment(fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>())
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

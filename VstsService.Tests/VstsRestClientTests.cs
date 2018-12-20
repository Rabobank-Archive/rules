using System;
using System.Net;
using Newtonsoft.Json.Linq;
using NSubstitute;
using RestSharp;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    public class VstsRestClientTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _vsts;

        public VstsRestClientTests(TestConfig config)
        {
            _config = config;
            _vsts = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void DeleteThrowsOnError()
        {
            var request = Substitute.For<IVstsRestRequest<int>>();
            var response = Substitute.For<IRestResponse>();
            
            var rest = Substitute.For<IRestClient>();
            rest.Execute(Arg.Any<IRestRequest>()).Returns(response);
            
            var client = new VstsRestClient("dummy", "pat", rest);            
            Assert.Throws<Exception>(() => client.Delete(request));
        }
        
        [Fact]
        public void PostThrowsOnError()
        {
            var request = Substitute.For<IVstsPostRequest<int>>();
            var response = Substitute.For<IRestResponse>();
            
            var rest = Substitute.For<IRestClient>();
            rest.Execute(Arg.Any<IRestRequest>()).Returns(response);
            
            var client = new VstsRestClient("dummy", "pat", rest);        
            Assert.Throws<Exception>(() => client.Post(request));
        }
        
        [Fact]
        public void GetThrowsOnError()
        {
            var request = Substitute.For<IVstsRestRequest<int>>();
            var response = Substitute.For<IRestResponse>();
            
            var rest = Substitute.For<IRestClient>();
            rest.Execute(Arg.Any<IRestRequest>()).Returns(response);
            
            var client = new VstsRestClient("dummy", "pat", rest);        
            Assert.Throws<Exception>(() => client.Get(request));
        }
        
        [Fact]
        public void GetJsonThrowsOnError()
        {
            var request = Substitute.For<IVstsRestRequest<JObject>>();
            var response = Substitute.For<IRestResponse>();
            
            var rest = Substitute.For<IRestClient>();
            rest.Execute(Arg.Any<IRestRequest>()).Returns(response);
            
            var client = new VstsRestClient("dummy", "pat", rest);        
            Assert.Throws<Exception>(() => client.Get(request));
        }
        
        [Fact]
        [Trait("category", "integration")]
        public void RestRequestResultAsJsonObject()
        {
            var endpoints = _vsts.Get(Requests.ServiceEndpoint.Endpoints(_config.Project).AsJson());
            endpoints.SelectToken("value[?(@.data.subscriptionId == '45cfa52a-a2aa-4a18-8d3d-29896327b51d')]").ShouldNotBeNull();
        }
    }
}
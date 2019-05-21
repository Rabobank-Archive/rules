using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using NSubstitute;
using RestSharp;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    public class VstsRestClientTests : IClassFixture<TestConfig>
    {
        private const string InvalidToken = "77p7fc7hpclqst4irzpwz452gkze75za7xkpbamkdy6lgtngjvcq";
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
            var request = Substitute.For<IVstsRequest<int>>();
            var response = Substitute.For<IRestResponse>();

            var rest = Substitute.For<IRestClient>();
            rest.Execute(Arg.Any<IRestRequest>()).Returns(response);

            var client = new VstsRestClient("dummy", "pat", rest);
            Assert.Throws<VstsException>(() => client.Delete(request));
        }

        [Fact]
        public void PostThrowsOnError()
        {
            var request = Substitute.For<IVstsRequest<int,int>>();
            var response = Substitute.For<IRestResponse>();

            var rest = Substitute.For<IRestClient>();
            rest.Execute(Arg.Any<IRestRequest>()).Returns(response);

            var client = new VstsRestClient("dummy", "pat", rest);
            Assert.Throws<VstsException>(() => client.Post(request, 3));
        }

        [Fact]
        public void GetThrowsOnError()
        {
            var request = Substitute.For<IVstsRequest<int>>();
            var response = Substitute.For<IRestResponse>();

            var rest = Substitute.For<IRestClient>();
            rest.Execute(Arg.Any<IRestRequest>()).Returns(response);

            var client = new VstsRestClient("dummy", "pat", rest);
            Assert.Throws<VstsException>(() => client.Get(request));
        }

        [Fact]
        public void GetJsonThrowsOnError()
        {
            var request = Substitute.For<IVstsRequest<JObject>>();
            var response = Substitute.For<IRestResponse>();

            var rest = Substitute.For<IRestClient>();
            rest.Execute(Arg.Any<IRestRequest>()).Returns(response);

            var client = new VstsRestClient("dummy", "pat", rest);
            Assert.Throws<VstsException>(() => client.Get(request));
        }

        [Fact]
        public void HtmlInsteadOfXmlShouldThrow()
        {
            var sut = new VstsRestClient("somecompany-test", InvalidToken);

            Assert.Throws<VstsException>(() =>
            {
                var result = sut.Get(Requests.Project.Projects());
            });
        }

        [Fact]
        [Trait("category", "integration")]
        public void RestRequestResultAsJsonObject()
        {
            var endpoints = _vsts.Get(Requests.ServiceEndpoint.Endpoints(_config.Project).AsJson());
            endpoints.SelectToken("value[?(@.data.subscriptionId == '45cfa52a-a2aa-4a18-8d3d-29896327b51d')]").ShouldNotBeNull();
        }

        
        [Fact]
        public void NotFoundIsNull()
        {
            _vsts.Get(Requests.Builds.Build("TAS", "2342423")).ShouldBeNull();
        }
    }
}
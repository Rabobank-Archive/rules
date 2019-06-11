using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Flurl.Http;
using Flurl.Http.Testing;
using Newtonsoft.Json.Linq;
using NSubstitute;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
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
            var request = new VstsRequest<int>("/delete/some/data");

            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(status: 500);
                var client = new VstsRestClient("dummy", "pat");
                Assert.Throws<FlurlHttpException>(() => client.Delete(request));
            }
         }

        [Fact]
        public void PostThrowsOnError()
        {
            var request = new VstsRequest<int,int>("/get/some/data");

            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(status: 500);
                var client = new VstsRestClient("dummy", "pat");
                Assert.Throws<FlurlHttpException>(() => client.Post(request, 3));
            }
        }
        
        [Fact]
        public void PutThrowsOnError()
        {
            var request = new VstsRequest<int,int>("/put/some/data");

            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(status: 500);
                var client = new VstsRestClient("dummy", "pat");
                Assert.Throws<FlurlHttpException>(() => client.Put(request, 3));
            }
        }

        [Fact]
        public void GetThrowsOnError()
        {
            var request = new VstsRequest<int>("/get/some/data");

            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(status: 500);
                var client = new VstsRestClient("dummy", "pat");
                Assert.Throws<FlurlHttpException>(() => client.Get(request));
            }
        }

        [Fact]
        public void GetJsonThrowsOnError()
        {
            var request = new VstsRequest<JObject>("/get/some/data");

            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(status: 500);
                var client = new VstsRestClient("dummy", "pat");
                Assert.Throws<FlurlHttpException>(() => client.Get(request));
            }
        }

        [Fact]
        [Trait("category", "integration")]
        public void HtmlInsteadOfXmlShouldThrow()
        {
            var sut = new VstsRestClient("somecompany-test", InvalidToken);
            Assert.Throws<FlurlHttpException>(() => sut.Get(Requests.Project.Projects()).ToList());
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
using System;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Testing;
using Newtonsoft.Json.Linq;
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
        public async Task DeleteThrowsOnError()
        {
            var request = new VstsRequest<int>("/delete/some/data");

            using var httpTest = new HttpTest();
            httpTest.RespondWith(status: 500);
            var client = new VstsRestClient("dummy", "pat");
            await Assert.ThrowsAsync<FlurlHttpException>(async () => await client.DeleteAsync(request));
        }

        [Fact]
        public async Task PostThrowsOnError()
        {
            var request = new VstsRequest<int,int>("/get/some/data");

            using var httpTest = new HttpTest();
            httpTest.RespondWith(status: 500);
            var client = new VstsRestClient("dummy", "pat");
            await Assert.ThrowsAsync<FlurlHttpException>(async () => await client.PostAsync(request, 3));
        }
        
        [Fact]
        public async Task PutThrowsOnError()
        {
            var request = new VstsRequest<int,int>("/put/some/data");

            using var httpTest = new HttpTest();
            httpTest.RespondWith(status: 500);
            var client = new VstsRestClient("dummy", "pat");
            await Assert.ThrowsAsync<FlurlHttpException>(async () => await client.PutAsync(request, 3));
        }

        [Fact]
        public async Task GetRawUrl()
        {
            var url = new Uri("http://www.bla.nl");

            using var httpTest = new HttpTest();
            httpTest.RespondWith(status: 200, body: "{}");
            var client = new VstsRestClient("dummy", "token");
            await client.GetAsync<Response.Build>(url);
            httpTest.ShouldHaveCalled(url.ToString());
        }
        
        [Fact]
        public async Task GetThrowsOnError()
        {
            var request = new VstsRequest<int>("/get/some/data");

            using var httpTest = new HttpTest();
            httpTest.RespondWith(status: 500);
            var client = new VstsRestClient("dummy", "pat");
            await Assert.ThrowsAsync<FlurlHttpException>(async () => await client.GetAsync(request));
        }

        [Fact]
        public async Task GetJsonThrowsOnError()
        {
            var request = new VstsRequest<JObject>("/get/some/data");

            using var httpTest = new HttpTest();
            httpTest.RespondWith(status: 500);
            var client = new VstsRestClient("dummy", "pat");
            await Assert.ThrowsAsync<FlurlHttpException>(async () => await client.GetAsync(request));
        }
        
        [Fact]
        public void EmptyPatShouldNotFailEarlyAndNotThrowWithInvalidHtml() => 
            Assert.Throws<ArgumentNullException>(() => new VstsRestClient("somecompany-test", null));
        
        [Fact]
        public void EmptyOrganizationShouldFailEarlyAndNotWithNotFound() => 
            Assert.Throws<ArgumentNullException>(() => new VstsRestClient(null, "pat"));

        [Fact]
        [Trait("category", "integration")]
        public async Task RestRequestResultAsJsonObject()
        {
            var endpoints = await _vsts.GetAsync(Requests.ServiceEndpoint.Endpoints(_config.Project).Request.AsJson());
            endpoints.SelectToken("value[?(@.data.subscriptionId == '45cfa52a-a2aa-4a18-8d3d-29896327b51d')]").ShouldNotBeNull();
        }
        
        [Fact]
        public async Task NotFoundIsNull()
        {
            var result = await _vsts.GetAsync(Requests.Builds.Build("TAS", "2342423"));
            result.ShouldBeNull();
        }
    }
}
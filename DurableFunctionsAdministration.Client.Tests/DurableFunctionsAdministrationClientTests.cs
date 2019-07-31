using System;
using System.Threading.Tasks;
using DurableFunctionsAdministration.Client.Request;
using Flurl.Http;
using Flurl.Http.Testing;
using Xunit;

namespace DurableFunctionsAdministration.Client.Tests
{
    public class DurableFunctionsAdministrationClientTests
    {
        [Fact]
        public async Task RequestShouldIncludeRequiredParams()
        {
            var request = new RestRequest<int>("/get/some/data");

            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(status: 200, body: "10");
                var client = new DurableFunctionsAdministrationClient(new Uri("https://some-dummy-uri"), "dummyTaskHub",
                    "dummyCode");
                await client.GetAsync(request);
                httpTest.ShouldHaveCalled("https://some-dummy-uri/*?*taskHub=dummyTaskHub*");
                httpTest.ShouldHaveCalled("https://some-dummy-uri/*?*code=dummyCode*");
            }
        }
        
        [Fact]
        public async Task GetReturnsDeserializedInt()
        {
            var request = new RestRequest<int>("/get/some/data");

            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(status: 200, body: "10");
                var client = new DurableFunctionsAdministrationClient(new Uri("https://some-dummy-uri"), "dummyTaskHub",
                    "dummyCode");
                var response = await client.GetAsync(request);
                Assert.Equal(10, response);
            }
        }
        
        [Fact]
        public async Task GetReturnsDeserializedJson()
        {
            var request = new RestRequest<DummyObject>("/get/some/data");

            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(status: 200, body: "{ \"someproperty\": \"somevalue\"}");
                var client = new DurableFunctionsAdministrationClient(new Uri("https://some-dummy-uri"), "dummyTaskHub",
                    "dummyCode");
                var response = await client.GetAsync(request);
                Assert.NotNull(response);
                Assert.Equal("somevalue", response.Someproperty);
            }
        }
        
        [Fact]
        public async Task GetThrowsOnError()
        {
            var request = new RestRequest<int>("/get/some/data");

            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(status: 500);
                var client = new DurableFunctionsAdministrationClient(
                    new Uri("https://some-dummy-uri"), "dummyTaskHub", "dummyCode");
                await Assert.ThrowsAsync<FlurlHttpException>(async () => await client.GetAsync(request));
            }
        }

        [Fact]
        public async Task DeleteThrowsOnError()
        {
            var request = new RestRequest<int>("/delete/some/data");

            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(status: 500);
                var client = new DurableFunctionsAdministrationClient(
                    new Uri("https://some-dummy-uri"), "dummyTaskHub", "dummyCode");
                await Assert.ThrowsAsync<FlurlHttpException>(async () => await client.DeleteAsync(request));
            }
        }
    }

    public class DummyObject
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Someproperty { get; set; }
    }
}
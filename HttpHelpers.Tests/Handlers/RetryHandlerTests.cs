using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using HttpHelpers.Handlers;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace HttpHelpers.Tests.Handlers
{
    public class RetryHandlerTests
    {
        [Theory]
        [InlineData(HttpStatusCode.RequestTimeout)] // 408
        [InlineData(HttpStatusCode.InternalServerError)] // 500
        [InlineData(HttpStatusCode.BadGateway)] // 502
        [InlineData(HttpStatusCode.ServiceUnavailable)] // 503
        [InlineData(HttpStatusCode.GatewayTimeout)] // 504
        public async Task ShouldRetryOnRetryableStatusCodes(HttpStatusCode statusCode)
        {
            var url = "http://www.bla.com";
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, url)
                .Respond(statusCode);
            mockHttp.Expect(HttpMethod.Get, url)
                .Respond(HttpStatusCode.OK);

            using (var client = new HttpClient(new RetryHandler(mockHttp)))
            {
                (await client.GetAsync(url)).Dispose();
            }
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task ShouldNotRetryOnSuccess()
        {
            var url = "http://www.bla.com";
            var mockHttp = new MockHttpMessageHandler();
            var request = mockHttp.When(HttpMethod.Get, url)
                .Respond(HttpStatusCode.OK);

            using (var client = new HttpClient(new RetryHandler(mockHttp)))
            {
                (await client.GetAsync(url)).Dispose();
            }
            mockHttp.GetMatchCount(request).ShouldBe(1);
        }

        [Fact]
        public async Task ShouldRetryOnRefusedConnection()
        {
            var ex = new SocketException((int) SocketError.ConnectionRefused);

            var url = "http://www.bla.com";
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, url)
                .Throw(ex);
            mockHttp.Expect(HttpMethod.Get, url)
                .Respond(HttpStatusCode.OK);

            using (var client = new HttpClient(new RetryHandler(mockHttp)))
            {
                (await client.GetAsync(url)).Dispose();
            }
            mockHttp.VerifyNoOutstandingExpectation();
        }
        
        [Fact]
        public async Task ShouldRetryOnTimeout()
        {
            var ex = new TaskCanceledException();

            var url = "http://www.bla.com";
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, url)
                .Throw(ex);
            mockHttp.Expect(HttpMethod.Get, url)
                .Respond(HttpStatusCode.OK);

            using (var client = new HttpClient(new RetryHandler(mockHttp)))
            {
                (await client.GetAsync(url)).Dispose();
            }
            mockHttp.VerifyNoOutstandingExpectation();
        }
    }
}
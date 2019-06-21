using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using SecurePipelineScan.VstsService.Handlers;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
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
            
            var client = new HttpClient(new RetryHandler(mockHttp));
            await client.GetAsync(url);
            
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task ShouldNotRetryOnSuccess()
        {
            var url = "http://www.bla.com";
            var mockHttp = new MockHttpMessageHandler();
            var request = mockHttp.When(HttpMethod.Get, url)
                .Respond(HttpStatusCode.OK);
            
            var client = new HttpClient(new RetryHandler(mockHttp));
            await client.GetAsync(url);
            
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

            var client = new HttpClient(new RetryHandler(mockHttp));
            await client.GetAsync(url);

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

            var client = new HttpClient(new RetryHandler(mockHttp));
            await client.GetAsync(url);

            mockHttp.VerifyNoOutstandingExpectation();
        }
    }
}
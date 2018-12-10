using System;
using System.Net;
using NSubstitute;
using RestSharp;
using SecurePipelineScan.VstsService;
using Xunit;

namespace VstsService.Tests
{
    public class VstsRestClientTests
    {
        [Fact]
        public void DeleteThrowsOnError()
        {
            var request = Substitute.For<IVstsRestRequest<int>>();

            var response = Substitute.For<IRestResponse>();
            response.StatusCode = HttpStatusCode.NotFound;
            
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
            response.StatusCode = HttpStatusCode.NotFound;
            
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
            response.StatusCode = HttpStatusCode.NotFound;
            
            var rest = Substitute.For<IRestClient>();
            rest.Execute(Arg.Any<IRestRequest>()).Returns(response);
            
            var client = new VstsRestClient("dummy", "pat", rest);        
            Assert.Throws<Exception>(() => client.Get(request));
        }
    }
}
using NSubstitute;
using RestSharp;
using Shouldly;
using System.Net;
using SecurePipelineScan.VstsService.Response;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    public class RestResponseExtensionsTests
    {
        [Fact]
        public void IncludesContentInErrorMessage()
        {
            var response = Substitute.For<IRestResponse<int>>();
            response.IsSuccessful.Returns(false);
            response.ErrorMessage.Returns(null, new string[0]);
            response.Content.Returns("fail");
            response.StatusCode.Returns(HttpStatusCode.SeeOther);

            var ex = Assert.Throws<VstsException>(() => response.ThrowOnError());
            ex.Message.ShouldBe("fail");
        }

        [Fact]
        public void ThrowsOnErrorReturnsResponseWhenNoError()
        {
            var response = new RestResponse<int>
            {
                StatusCode = HttpStatusCode.OK,
                ResponseStatus = ResponseStatus.Completed,
                Data = 4
            };

            response.ThrowOnError().Data.ShouldBe(4);
        }

        [Fact]
        public void ThrowsOnStatusCode()
        {
            var response = Substitute.For<IRestResponse<int>>();
            response.IsSuccessful.Returns(false);

            Assert.Throws<VstsException>(() => response.ThrowOnError());
        }

        [Fact]
        public void ThrowsOn203()
        {
            var response = Substitute.For<IRestResponse<int>>();
            response.IsSuccessful.Returns(true);
            response.StatusCode.Returns(HttpStatusCode.NonAuthoritativeInformation);
            response.StatusDescription.Returns("Non-Authoritative Information");

            Assert.Throws<VstsException>(() => response.ThrowOnError());
        }

        [Fact]
        public void ThrowsOn203_NonGeneric()
        {
            var response = Substitute.For<IRestResponse>();
            response.IsSuccessful.Returns(true);
            response.StatusCode.Returns(HttpStatusCode.NonAuthoritativeInformation);
            response.StatusDescription.Returns("Non-Authoritative Information");

            Assert.Throws<VstsException>(() => response.ThrowOnError());
        }

        [Fact]
        public void ThrowsNothingOnNotFound()
        {
            var response = Substitute.For<IRestResponse<Release>>();
            response.IsSuccessful.Returns(false);
            response.StatusCode.Returns(HttpStatusCode.NotFound);

            response.ThrowOnError().Data.ShouldBeNull();
        }

        [Fact]
        public void ThrowsNothingOnNotFoundOnRequestWithNoResult()
        {
            var response = Substitute.For<IRestResponse>();
            response.IsSuccessful.Returns(false);
            response.StatusCode.Returns(HttpStatusCode.NotFound);

            response.ThrowOnError().Content.ShouldBeEmpty();
        }

        [Fact]
        public void DefaultIfNotFound()
        {
            var response = Substitute.For<IRestResponse<Build>>();
            response.StatusCode.Returns(HttpStatusCode.NotFound);

            response.DefaultIfNotFound().ShouldBeNull();
        }
        
        [Fact]
        public void DataIfFound()
        {
            var response = Substitute.For<IRestResponse<Build>>();
            response.StatusCode.Returns(HttpStatusCode.OK);
            response.Data.Returns(new Build());

            response.DefaultIfNotFound().ShouldNotBeNull();
        }
    }
}
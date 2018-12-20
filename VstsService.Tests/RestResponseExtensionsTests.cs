using NSubstitute;
using RestSharp;
using Shouldly;
using System;
using System.Net;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    public class RestResponseExtensionsTests
    {
        [Fact]
        public void IncludesErrorMessageWhenPresent()
        {
            var response = Substitute.For<IRestResponse<int>>();
            response.IsSuccessful.Returns(false);
            response.ErrorMessage.Returns("fail");

            var ex = Assert.Throws<Exception>(() => response.ThrowOnError());
            ex.Message.ShouldBe("fail");
        }

        [Fact]
        public void IncludesContentOtherwise()
        {
            var response = Substitute.For<IRestResponse<int>>();
            response.IsSuccessful.Returns(false);
            response.ErrorMessage.Returns(null, new string[0]);
            response.Content.Returns("fail");

            var ex = Assert.Throws<Exception>(() => response.ThrowOnError());
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

            Assert.Throws<Exception>(() => response.ThrowOnError());
        }

        [Fact]
        public void ThrowsOn203()
        {
            var response = Substitute.For<IRestResponse<int>>();
            response.IsSuccessful.Returns(true);
            response.StatusCode.Returns(HttpStatusCode.NonAuthoritativeInformation);
            response.StatusDescription.Returns("Non-Authoritative Information");

            Assert.Throws<Exception>(() => response.ThrowOnError());
        }

        [Fact]
        public void ThrowsOn203_NonGeneric()
        {
            var response = Substitute.For<IRestResponse>();
            response.IsSuccessful.Returns(true);
            response.StatusCode.Returns(HttpStatusCode.NonAuthoritativeInformation);
            response.StatusDescription.Returns("Non-Authoritative Information");

            Assert.Throws<Exception>(() => response.ThrowOnError());
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
    }
}
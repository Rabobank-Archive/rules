using System;
using System.Threading.Tasks;
using RestSharp;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    public class RestResponseExtensionsTests
    {
        [Fact]
        public void ThrowOnErrorAsync()
        {
            var response = new RestResponse<int>
            {
                ErrorMessage = "fail"
            };

            var ex = Assert.Throws<Exception>(() => response.ThrowOnError());
            ex.Message.ShouldBe("fail");
        }

        [Fact]
        public void ThrowsOnErrorReturnsResponseWhenNoError()
        {
            var response = new RestResponse<int>
            {
                Data = 4
            };

            response.ThrowOnError().Data.ShouldBe(4);
        }
    }
}
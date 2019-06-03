using System;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    public class RestClientFactoryTests
    {
        [Fact]
        public void UniqueClientPerBaseUrl()
        {
            var factory = new RestClientFactory();
            factory
                .Create(new Uri("https://a"))
                .ShouldNotBeSameAs(
                    factory.Create(new Uri("https://b")));
        }

        [Fact]
        public void SameClientForSameUrl()
        {
            var factory = new RestClientFactory();
            factory
                .Create(new Uri("https://a"))
                .ShouldBeSameAs(
                    factory.Create(new Uri("https://a")));
        }
    }
}
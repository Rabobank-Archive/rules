using System;
using AutoFixture;
using NSubstitute;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    public class VstsRestRequestExtensionsTests 
    {
        [Fact]
        public void RestRequestAsJson()
        {
            var fixture = new Fixture();
            var request = Substitute.For<IVstsRequest<int>>();
            
            var uri = fixture.Create<string>();
            request.Uri.Returns(uri);

            var baseUri = fixture.Create<Uri>();
            request.BaseUri(Arg.Any<string>()).Returns(baseUri);

            var target = request.AsJson();
            target.Uri.ShouldBe(uri);
            target.BaseUri(fixture.Create<string>()).ShouldBe(baseUri);
        }
    }
}
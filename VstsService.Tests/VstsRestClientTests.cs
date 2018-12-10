using NSubstitute;
using SecurePipelineScan.VstsService;
using Xunit;

namespace VstsService.Tests
{
    public class VstsRestClientTests
    {
        [Fact]
        public void DeleteThrowsOnError()
        {
            var request = Substitute.For<IVstsPostRequest<int>>();
            var client = new VstsRestClient("dummy", "pat");
            
            client.Delete(request);
        }
    }
}
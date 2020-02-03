using System.Threading.Tasks;
using Flurl.Http.Testing;
using SecurePipelineScan.Rules.Security.Cmdb.Client;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    public class CmdbClientTests
    {
        private HttpTest _httpTest = new HttpTest();
        private void Dispose()
        {
            _httpTest.Dispose();
        }

        [Fact]
        public async Task TestGetUserEntitlement()
        {
            var client = new CmdbClient(new CmdbClientConfig("abc", "http://localhost/", "somecompany", "CI1234"));
            var result = await client.GetCiAsync("1234");

            _httpTest.ShouldHaveCalled("http://localhost/devices?CiIdentifier=1234&view=expand");
        }

        [Fact]
        public async Task TestGetAssignment()
        {
            var client = new CmdbClient(new CmdbClientConfig("abc", "http://localhost/", "somecompany", "CI1234"));
            var result = await client.GetAssignmentAsync("TAS");

            _httpTest.ShouldHaveCalled("http://localhost/assignments?name=TAS&view=expand");
        }
    }
}
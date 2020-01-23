using System.Threading.Tasks;
using Flurl.Http.Testing;
using SecurePipelineScan.VstsService.Requests;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    public class UserEntitlementTests : IClassFixture<TestConfig>
    {
        private HttpTest _httpTest = new HttpTest();
        private readonly IVstsRestClient _client;

        public UserEntitlementTests(TestConfig config)
        {
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        private void Dispose()
        {
            _httpTest.Dispose();
        }

        [Fact]
        public async Task TestGetUserEntitlement()
        {
            var result = await _client.GetAsync(MemberEntitlementManagement.GetUserEntitlement("abc"));

            _httpTest.ShouldHaveCalled("https://vsaex.dev.azure.com/somecompany-test/_apis/UserEntitlements/abc?api-version=5.0-preview.2");
        }
    }
}
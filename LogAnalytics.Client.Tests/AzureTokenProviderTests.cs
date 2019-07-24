using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace LogAnalytics.Client.Tests
{
    public class AzureTokenProviderTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public AzureTokenProviderTests(TestConfig config)
        {
            _config = config;
        }
        
        [Fact]
        public async Task GetAccessTokenReturnsToken()
        {
            var tokenprovider = new AzureTokenProvider(_config.TenantId, _config.ClientId, _config.ClientSecret);
            var token = await tokenprovider.GetAccessTokenAsync();
            token.ShouldNotBeNull();
        }
    }
}
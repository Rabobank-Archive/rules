using System.Linq;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    public class UserEntitlementTests : IClassFixture<TestConfig>
    {
        private readonly IVstsRestClient _client;

        public UserEntitlementTests(TestConfig config)
        {
            _client = new VstsRestClient(config.Organization, config.Token);
        }
        
        [Fact]
        public void TestUserEntitlement()
        {
            var result = _client
                .Get(MemberEntitlementManagement.UserEntitlements());

            var first = result.First(x => x.LastAccessedDate != default);
            first.DateCreated.ShouldNotBe(default);
            first.Id.ShouldNotBe(default);

            var user = first.User;
            user.PrincipalName.ShouldNotBe(default);
            user.MailAddress.ShouldNotBe(default);
            user.DisplayName.ShouldNotBe(default);

            var msdn = result.First(x => x.AccessLevel.LicensingSource == "msdn").AccessLevel;
            msdn.Status.ShouldNotBeEmpty();
            msdn.LicenseDisplayName.ShouldNotBeEmpty();
            msdn.MsdnLicenseType.ShouldNotBe("none");
            msdn.AccountLicenseType.ShouldBe("none");
            
            var account = result.First(x => x.AccessLevel.LicensingSource == "account").AccessLevel;
            account.Status.ShouldNotBeEmpty();
            account.MsdnLicenseType.ShouldBe("none");
            account.AccountLicenseType.ShouldNotBe("none");
            account.MsdnLicenseType.ShouldNotBeEmpty();
        }

        [Fact]
        public void TestMultipleEntitlements_WhenResultIsMoreThanTake_ThenRemainderShouldFetchedInSubsequentRequest()
        {
            var result = _client.Get(MemberEntitlementManagement.UserEntitlements(), 20);
            result.Count().ShouldBeGreaterThan(20);
        }
    }
}
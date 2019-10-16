using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
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
                .Get(MemberEntitlementManagement.UserEntitlements())
                .ToList();

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
            var result = _client.Get(MemberEntitlementManagement.UserEntitlements());
            result.Count().ShouldBeGreaterThan(20);
        }

        [Theory]
        [InlineData("stakeholder")]
        [InlineData("express")]
        public async Task TestUpdateLicense(string license)
        {
            const string user = "FU.Tasportal_vsts@somecompany.nl";
            
            var entitlement = _client
                .Get(MemberEntitlementManagement.UserEntitlements())
                .FirstOrDefault(e => e.User.MailAddress.Equals(user));

            entitlement.AccessLevel.AccountLicenseType = license;
            
            var patchDocument = new JsonPatchDocument().Replace("/accessLevel", entitlement.AccessLevel);
            _ = await _client.PatchAsync(MemberEntitlementManagement.PatchUserEntitlements(entitlement.Id), patchDocument);

            var result = _client
                .Get(MemberEntitlementManagement.UserEntitlements())
                .FirstOrDefault(e => e.User.MailAddress.Equals(user));

            Assert.Equal(license, result.AccessLevel.AccountLicenseType);
        }

        [Fact]
        public async Task TestUserEntitlementSummary()
        {
            var result = await _client.GetAsync(MemberEntitlementManagement.UserEntitlementSummary());
            result.Licenses.ShouldNotBeEmpty();

            var license = result.Licenses.First();
            license.LicenseName.ShouldNotBeEmpty();
            license.Assigned.ShouldNotBe(default);
        }
    }
}
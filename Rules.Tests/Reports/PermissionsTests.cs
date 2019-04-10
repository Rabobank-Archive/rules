using Common;
using Rules.Reports;
using SecurePipelineScan.Rules.Reports;
using Shouldly;
using Xunit;
using static Common.PermissionId;
using Permission = Common.Permission;

namespace SecurePipelineScan.Rules.Tests.Reports
{
    public class PermissionsTests
    {
        [Theory]
        [InlineData(Deny, Deny, true)]
        [InlineData(Allow, AllowInherited, true)]
        [InlineData(AllowInherited, Allow, true)]
        [InlineData(Deny, DenyInherited, true)]
        [InlineData(DenyInherited, Deny, true)]
        [InlineData(Deny, Allow, false)]

        public void PermissionIsCompliantShouldBeTrue(Common.PermissionId actualPermissionId, Common.PermissionId shouldBePermissionId, bool isCompliant)
        {
            var permission =  new global::Rules.Reports.Permission(3, actualPermissionId);
            permission.ShouldBePermissionId = shouldBePermissionId;

            permission.IsCompliant.ShouldBe(isCompliant);

        }

        [Fact]
        public void NoShouldBePermissionId_ShouldBeCompliant()
        {
            var permission = new global::Rules.Reports.Permission(8);
            permission.ShouldBePermissionId.ShouldBeNull();
            permission.IsCompliant.ShouldBeTrue();
        }

    }
}
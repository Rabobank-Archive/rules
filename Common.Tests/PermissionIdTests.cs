using Common;
using Xunit;

namespace SecurePipelineScan.Common.Tests
{
    public class PermissionIdTests
    {

        [Fact]
        public void PermissionIdShouldUseFriendlyDisplayNameWhenUsingGetDisplayName()
        {
            Assert.Equal("Not Set", PermissionId.NotSet.GetDisplayName());
            Assert.Equal("Deny", PermissionId.Deny.GetDisplayName());
            Assert.Equal("Deny (inherited)", PermissionId.DenyInherited.GetDisplayName());
            Assert.Equal("Allow", PermissionId.Allow.GetDisplayName());
            Assert.Equal("Allow (inherited)", PermissionId.AllowInherited.GetDisplayName());
            
        }
    }
}
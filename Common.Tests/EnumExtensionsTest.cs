using System;
using System.ComponentModel.DataAnnotations;
using Common;
using Xunit;

namespace SecurePipelineScan.Common.Tests
{
    public class EnumExtensionsTest
    {
        [Fact]
        public void GetDisplayNameWithNullValueAsInputShouldReturnEmptyString()
        {
            Assert.Empty(EnumExtensions.GetDisplayName(null));
        }

        [Fact]
        public void GetDisplayNameWithEnumShouldReturnDisplayName()
        {
            Assert.Equal("Not Set", PermissionId.NotSet.GetDisplayName());
            Assert.Equal("Deny", PermissionId.Deny.GetDisplayName());
            Assert.Equal("Deny (inherited)", PermissionId.DenyInherited.GetDisplayName());
            Assert.Equal("Allow", PermissionId.Allow.GetDisplayName());
            Assert.Equal("Allow (inherited)", PermissionId.AllowInherited.GetDisplayName());
        }

    }
}
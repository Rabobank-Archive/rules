using System.Collections.Generic;
using Rules.Reports;
using Shouldly;
using Xunit;
using static Common.PermissionId;
using Permission = Rules.Reports.Permission;

namespace SecurePipelineScan.Rules.Tests.Reports
{
    public class GlobalPermissionsTest
    {
        [Fact]
        public void GlobalPermissionsWithoutShouldBePermissionIds_ShouldBeCompliant()
        {
            var globalPermission = new GlobalPermissions()
            {
                ApplicationGroupName = "groupName1",
                Permissions = new List<Permission>()
                {
                    new Permission(32)
                    {
                        ActualPermissionId = Allow
                    },
                    new Permission(64)
                    {
                        ActualPermissionId = Deny
                    }
                }
            };
            globalPermission.IsCompliant.ShouldBeTrue();
        }

        [Fact]
        public void GlobalPermissionsWithCompliantPermissions_ShouldBeCompliant()
        {
            var globalPermission = new GlobalPermissions()
            {
                ApplicationGroupName = "groupName1",
                Permissions = new List<Permission>()
                {
                    new Permission(32)
                    {
                        ShouldBePermissionId = Allow,
                        ActualPermissionId = Allow
                    },
                    new Permission(64)
                    {
                        ShouldBePermissionId = Allow,
                        ActualPermissionId = Allow
                    }
                }
            };
            globalPermission.IsCompliant.ShouldBeTrue();

        }

        [Fact]
        public void GlobalPermissionsWithNotCompliantPermission_ShouldNotBeCompliant()
        {
            var globalPermission = new GlobalPermissions()
            {
                ApplicationGroupName = "groupName1",
                Permissions = new List<Permission>()
                {
                    new Permission(32)
                    {
                        ShouldBePermissionId = Allow,
                        ActualPermissionId = Deny
                    },
                    new Permission(64)
                    {
                        ShouldBePermissionId = Allow,
                        ActualPermissionId = Allow
                    }
                }
            };
            globalPermission.IsCompliant.ShouldBeFalse();

        }
    }
}
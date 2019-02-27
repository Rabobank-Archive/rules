using System;
using System.Collections.Generic;
using System.Linq;
using Rules.Reports;
using SecurePipelineScan.Rules.Reports;
using Shouldly;
using Xunit;
using static Common.PermissionId;
using Permission = Rules.Reports.Permission;

namespace SecurePipelineScan.Rules.Tests.Reports
{
    public class SecurityReportTests
    {
        [Fact]
        public void EmptySecurityReportIsCompliant()
        {
            var securityReport = new SecurityReport(DateTime.UtcNow);
            securityReport.IsCompliant.ShouldBeTrue();
        }

        [Fact]
        public void CompliantPermissionsSecurityReportShouldBeCompliant()
        {

            var securityReport = new SecurityReport(DateTime.UtcNow);
            MockCompliantGlobalPermissions(securityReport); 
            
            securityReport.GlobalPermissions.All(p => p.IsCompliant).ShouldBeTrue();
            securityReport.IsCompliant.ShouldBeTrue();
        }
        
        [Fact]
        public void NotCompliantPermissionsSecurityReportShouldNotBeCompliant()
        {

            var securityReport = new SecurityReport(DateTime.UtcNow);
            MockNotCompliantGlobalPermissions(securityReport); 
            
            securityReport.GlobalPermissions.All(p => p.IsCompliant).ShouldBeFalse();
            securityReport.IsCompliant.ShouldBeFalse();
        }


        public static void MockCompliantGlobalPermissions(SecurityReport securityReport)
        {
            securityReport.GlobalPermissions = new GlobalPermissions[]
            {
                new GlobalPermissions()
                {
                    ApplicationGroupName = "groupName1",
                    Permissions = new List<Permission>()
                    {
                        new Permission(2)
                        {
                            ActualPermissionId = Allow,
                            ShouldBePermissionId = Allow
                        },
                        new Permission(4)
                        {
                            ActualPermissionId = Allow,
                            ShouldBePermissionId = Allow
                        }
                    }
                },
                new GlobalPermissions()
                {
                    ApplicationGroupName = "groupName2",
                    Permissions = new List<Permission>()
                    {
                        new Permission(8)
                        {
                            ActualPermissionId = Deny,
                            ShouldBePermissionId = Deny
                        },
                    }

                }
            };
        }
        
        public static void MockNotCompliantGlobalPermissions(SecurityReport securityReport)
        {
            securityReport.GlobalPermissions = new GlobalPermissions[]
            {
                new GlobalPermissions()
                {
                    ApplicationGroupName = "groupName1",
                    Permissions = new List<Permission>()
                    {
                        new Permission(2)
                        {
                            ActualPermissionId = Allow,
                            ShouldBePermissionId = Deny
                        },
                        new Permission(4)
                        {
                            ActualPermissionId = Allow,
                            ShouldBePermissionId = Allow
                        }
                    }
                },
                new GlobalPermissions()
                {
                    ApplicationGroupName = "groupName2",
                    Permissions = new List<Permission>()
                    {
                        new Permission(8)
                        {
                            ActualPermissionId = Deny,
                            ShouldBePermissionId = Allow
                        },
                    }

                }
            };
        }
    }
}
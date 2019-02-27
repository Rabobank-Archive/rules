using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Rules.Reports;
using SecurePipelineScan.Rules.Pdf;
using SecurePipelineScan.Rules.Reports;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using Xunit;
using static Common.PermissionId;
using Permission = Rules.Reports.Permission;

namespace SecurePipelineScan.Rules.Tests.Pdf
{
    public class PdfServiceTests
    {
        [Fact]
        public void CreateReport()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var report = fixture.Create<SecurityReport>();
            report.ProjectName = "fakeProjectName";
            report.GlobalPermissions = new GlobalPermissions[]
            {
                new GlobalPermissions()
                {
                    ApplicationGroupName = "Production Environment Owner",
                    Permissions = new List<Permission>()
                    {
                        new Permission(1,DenyInherited) { ShouldBePermissionId = Deny, Description = "Permanently Delete Work Items"},
                        new Permission(2,Deny) { ShouldBePermissionId = Deny, Description = "Manage Project Properties"},
                        new Permission(3,NotSet) { Description = "Rename team project"},
                    }
                },
                new GlobalPermissions()
                {
                    ApplicationGroupName = "Project Administrators",
                    Permissions = new List<Permission>()
                    {
                        new Permission(1,AllowInherited) { ShouldBePermissionId = Allow, Description = "Permanently Delete Work Items"},
                        new Permission(2,Deny) { ShouldBePermissionId = Allow, Description = "Manage Project Properties"},
                        new Permission(3,Allow) { Description = "Rename team project"},
                    }
                },
                new GlobalPermissions()
                {
                    ApplicationGroupName = "Build Administrators",
                    Permissions = new List<Permission>()
                    {
                        new Permission(1) { ShouldBePermissionId = Deny, Description = "Permanently Delete Work Items"},
                        new Permission(2,Deny) { ShouldBePermissionId = Deny, Description = "Manage Project Properties"},
                        new Permission(3,Allow) { Description = "Rename team project"},
                    }
                }
            };

            var sut = new PdfService("Security_Reports");

            sut.CreateReport(report.ProjectName, report);

            File.Exists(Path.Combine("Security_Reports", report.ProjectName + ".pdf")).ShouldBeTrue();
        }
    }
}
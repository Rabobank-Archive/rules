using Rules.Reports;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Reports
{
    public class SecurityReportExtensionsTests
    {
        [Fact]
        public void MapShouldBeEmpty()
        {
            var report = new Rules.Reports.SecurityReport(DateTime.UtcNow);

            var result = Rules.Reports.SecurityReportExtensions.Map(report);

            result.ShouldBeOfType<ProjectOverviewData>();
        }

        [Fact]
        public void MapShouldHaveReportData()
        {
            var sut = new Rules.Reports.SecurityReport(DateTime.UtcNow);

            sut.GlobalPermissions = new[] {
                new GlobalPermissions
                {
                    ApplicationGroupName = "actualGroup1",
                    Permissions = new[]
                    {
                        new Permission(1,Common.PermissionId.Allow)
                        {
                            ShouldBePermissionId = Common.PermissionId.Deny,
                        }
                    }
                }
            };

            var result = Rules.Reports.SecurityReportExtensions.Map(sut);

            result.Namespaces.First().name.ShouldBe("Global Project Permissions");
            result.Namespaces.First().applicationGroups.First().name.ShouldBe("actualGroup1");

            result.Namespaces.First().isCompliant.ShouldBeFalse();
            result.Namespaces.First().applicationGroups.First().isCompliant.ShouldBeFalse();

        }
    }
}
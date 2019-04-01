using Rules.Reports;
using SecurePipelineScan.VstsService.Requests;
using System.Collections.Generic;
using System.Linq;

namespace SecurePipelineScan.Rules.Reports
{
    public static class SecurityReportExtensions
    {
        public static ProjectOverviewData Map(this SecurityReport report)
        {
            return new ProjectOverviewData()
            {
                Etag = -1,
                Id = report.ProjectName,
                Namespaces = CreateNamespaces(report),
            };
        }

        private static IEnumerable<ExtensionManagement.Namespace> CreateNamespaces(SecurityReport report)
        {
            var nameSpace = new ExtensionManagement.Namespace
            {
                name = "Global Project Permissions",
                description = "Global Project Permissions",
                key = "key",
                applicationGroups = CreateApplicationGroups(report.GlobalPermissions),
            };
            nameSpace.isCompliant = nameSpace.applicationGroups.All(x => x.isCompliant);
            yield return nameSpace;
        }

        private static IEnumerable<ExtensionManagement.ApplicationGroup> CreateApplicationGroups(IEnumerable<GlobalPermissions> globalPermissions)
        {
            foreach (var item in globalPermissions)
            {
                var applicationGroup = new ExtensionManagement.ApplicationGroup
                {
                    name = item.ApplicationGroupName,
                    permissions = CreatePermissions(item.Permissions),
                };

                applicationGroup.isCompliant = applicationGroup.permissions.All(x => x.IsCompliant);
                yield return applicationGroup;
            }
        }

        private static IEnumerable<ExtensionManagement.Permission> CreatePermissions(IList<Permission> permissions)
        {
            foreach (var item in permissions)
            {
                yield return new ExtensionManagement.Permission
                {
                    ActualValue = (int)(item.ActualPermissionId ?? 0),
                    Bit = item.Description,
                    ShouldBe = (int)(item.ShouldBePermissionId ?? 0),
                    IsCompliant = item.IsCompliant,
                };
            }
        }
    }
}
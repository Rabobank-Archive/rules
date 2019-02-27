using Common;
using Rules.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using Permission = Rules.Reports.Permission;

namespace SecurePipelineScan.Rules.Reports
{
    internal class SecurityReportProcessor
    {
        internal SecurityReport Evaluate(ISecurityData securityData, ICompliantValues compliantValues)
        {
            var report = new SecurityReport(DateTime.UtcNow);

            report.ProjectName = securityData.ProjectName;

            var globalPermissions = MapGlobalPermissions(securityData).ToList();

            InsertOrUpdateCompliantValues(compliantValues, globalPermissions);

            report.GlobalPermissions = globalPermissions;

            return report;
        }

        private static void InsertOrUpdateCompliantValues(ICompliantValues compliantValues, IList<GlobalPermissions> globalPermissions)
        {
            foreach (var compliantPermission in compliantValues.GlobalPermissions)
            {
                InsertOrUpdateGlobalPermissions(globalPermissions, compliantPermission);
            }
        }

        private static void InsertOrUpdateGlobalPermissions(IList<GlobalPermissions> globalPermissions, KeyValuePair<string, IEnumerable<Common.Permission>> compliantPermission)
        {
            var foundGlobalPermission = globalPermissions.
                SingleOrDefault(x => x.ApplicationGroupName == compliantPermission.Key);

            if (foundGlobalPermission != null)
            {
                foreach (var compliantPermissionEntry in compliantPermission.Value)
                {
                    InsertOrUpdateGlobalPermissionEntry(foundGlobalPermission, compliantPermissionEntry);
                }
            }
            else
            {
                globalPermissions.Add(new GlobalPermissions()
                {
                    ApplicationGroupName = compliantPermission.Key,
                    Permissions = compliantPermission.Value.
                    Select(x => new Permission(x.PermissionBit)
                    {
                        ShouldBePermissionId = x.PermissionId
                    }
                    ).ToList()
                });
            }
        }

        private static void InsertOrUpdateGlobalPermissionEntry(GlobalPermissions foundGlobalPermission, Common.Permission compliantPermissionEntry)
        {
            var foundGlobalPermissionEntry =
                foundGlobalPermission.Permissions.
                SingleOrDefault(x => x.PermissionBit == compliantPermissionEntry.PermissionBit);

            if (foundGlobalPermissionEntry != null)
            {
                foundGlobalPermissionEntry.ShouldBePermissionId =
                    compliantPermissionEntry.PermissionId;
            }
            else
            {
                foundGlobalPermission.Permissions.Add(
                    new Permission(compliantPermissionEntry.PermissionBit)
                    {
                        ShouldBePermissionId = compliantPermissionEntry.PermissionId
                    });
            }
        }

        private static IEnumerable<GlobalPermissions> MapGlobalPermissions(ISecurityData data)
        {
            foreach (var globalPermission in data.GlobalPermissions)
            {
                yield return new GlobalPermissions
                {
                    ApplicationGroupName = globalPermission.Key,
                    Permissions = MapPermissionEntry(globalPermission).ToList()
                };
            }
        }

        private static IEnumerable<Permission> MapPermissionEntry(KeyValuePair<string, IEnumerable<Common.Permission>> globalPermission)
        {
            foreach (var inputPermission in globalPermission.Value)
            {
                yield return new Permission(inputPermission.PermissionBit, inputPermission.PermissionId);
            }
        }
    }
}
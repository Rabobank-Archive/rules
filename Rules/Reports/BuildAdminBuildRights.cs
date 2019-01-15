using Rules.Reports;

namespace SecurePipelineScan.Rules.Reports
{
    public class BuildAdminBuildRights : BuildRights
    {
        public override bool IsSecure =>
            HasNoPermissionsToDeleteBuilds && 
            HasNoPermissionsToDeleteBuildDefinition &&
            HasNoPermissionsToDeDestroyBuilds && 
            HasNoPermissionsToAdministerBuildPermissions;
    }
}
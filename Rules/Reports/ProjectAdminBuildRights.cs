namespace SecurePipelineScan.Rules.Reports
{
    public class ProjectAdminBuildRights : BuildRights
    {
        public override bool IsSecure =>
            HasNoPermissionsToDeleteBuilds && 
            HasNoPermissionsToDeleteBuildDefinition &&
            HasNoPermissionsToDestroyBuilds && 
            HasNoPermissionsToAdministerBuildPermissions;
    }
}
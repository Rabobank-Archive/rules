namespace SecurePipelineScan.Rules.Reports
{
    public class ContributorsBuildRights : BuildRights
    {
        public override bool IsSecure =>
            HasNoPermissionsToDeleteBuildDefinition && 
            HasNoPermissionsToDeleteBuilds &&
            HasNoPermissionsToDestroyBuilds && 
            HasNoPermissionsToAdministerBuildPermissions;
    }
}
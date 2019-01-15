namespace SecurePipelineScan.Rules.Reports
{
    public class RaboAdminReleaseRights : ReleaseRights
    {
        public override bool IsSecure =>
            HasNoPermissionsToAdministerReleasePermissions &&
            HasNoPermissionToDeleteReleasePipeline &&
            HasNoPermissionToDeleteReleases &&
            HasNoPermissionToManageReleaseApprovers;
    }
}
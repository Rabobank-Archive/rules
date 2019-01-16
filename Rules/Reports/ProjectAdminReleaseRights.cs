namespace SecurePipelineScan.Rules.Reports
{
    public class ProjectAdminReleaseRights : ReleaseRights
    {
        public override bool IsSecure =>
            HasNoPermissionsToAdministerReleasePermissions &&
            HasNoPermissionToDeleteReleasePipeline &&
            HasNoPermissionToDeleteReleaseStage &&
            HasNoPermissionToDeleteReleases &&
            HasNoPermissionToManageReleaseApprovers;
    }
}
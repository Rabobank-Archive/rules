namespace SecurePipelineScan.Rules.Reports
{
    public class ContributorsReleaseRights : ReleaseRights
    {
        public override bool IsSecure =>
            HasNoPermissionsToAdministerReleasePermissions &&
            HasNoPermissionToDeleteReleasePipeline &&
            HasNoPermissionToDeleteReleases &&
            HasNotSetToManageReleaseApprovers &&
            HasNotSetToDeleteReleaseStage;
    }
}
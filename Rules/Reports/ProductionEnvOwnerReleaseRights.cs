namespace SecurePipelineScan.Rules.Reports
{
    public class ProductionEnvOwnerReleaseRights : ReleaseRights
    {
        public override bool IsSecure =>
            HasPermissionToManageReleaseApprovers &&
            HasNoPermissionToCreateReleases &&
            HasPermissionToDeleteReleaseStage;
    }
}
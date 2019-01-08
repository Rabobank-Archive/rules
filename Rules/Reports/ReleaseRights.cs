namespace SecurePipelineScan.Rules.Reports
{
    public class ReleaseRights
    {
        public bool HasNoPermissionsToAdministerReleasePermissions { set; get; }
        public bool HasNoPermissionToDeleteReleasePipeline { set; get; }
        public bool HasNoPermissionToDeleteReleases { set; get; }
        public bool HasNoPermissionToManageReleaseApprovers { set; get; }
        public bool HasNoPermissionToCreateReleases { set; get; }

        public bool ReleaseRightsIsSecure => HasNoPermissionsToAdministerReleasePermissions &&
                                             HasNoPermissionToDeleteReleasePipeline &&
                                             HasNoPermissionToDeleteReleases &&
                                             HasNoPermissionToManageReleaseApprovers &&
                                             HasNoPermissionToCreateReleases;
    }

    public class ReleaseRightsProductionEnvOwner
    {
        public bool HasPermissionToManageReleaseApprovers { set; get; }
        public bool HasNoPermissionToCreateReleases { set; get; }

        public bool ReleaseRightsProductEnvOwnerIsSecure => HasPermissionToManageReleaseApprovers &&
                                                            HasNoPermissionToCreateReleases;
    }
}
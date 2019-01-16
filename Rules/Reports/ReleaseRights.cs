namespace SecurePipelineScan.Rules.Reports
{
    public abstract class ReleaseRights
    {
        public bool HasNoPermissionsToAdministerReleasePermissions { set; get; }
        public bool HasNoPermissionToDeleteReleasePipeline { set; get; }
        public bool HasNoPermissionToDeleteReleases { set; get; }
        public bool HasNoPermissionToManageReleaseApprovers { set; get; }
        public bool HasPermissionToManageReleaseApprovers { set; get; }
        public bool HasNotSetToManageReleaseApprovers { get; set; }
        public bool HasNoPermissionToCreateReleases { set; get; }
        public bool HasPermissionToCreateReleases { get; set; }
        public bool HasNoPermissionToDeleteReleaseStage { get; set; }
        public bool HasNotSetToDeleteReleaseStage { get; set; }

        public abstract bool IsSecure { get; }
    }
}
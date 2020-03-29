namespace SecurePipelineScan.Rules.PermissionBits
{
    public static class Release
    {
        public const int ViewReleasePipeline = 1;
        public const int EditReleasePipeline = 2;
        public const int DeleteReleasePipelines = 4;
        public const int ManageApprovalsPermissionBit = 8;
        public const int CreateReleasesPermissionBit = 64;
        public const int EditReleaseStage = 128;
        public const int AdministerReleasePermissions = 512;
        public const int DeleteReleases = 1024;
    }
}
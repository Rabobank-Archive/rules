namespace SecurePipelineScan.Rules.PermissionBits
{
    public static class Release
    {
        public const int DeleteReleasePipelines = 4;
        public const int ManageApprovalsPermissionBit = 8;
        public const int CreateReleasesPermissionBit = 64;
        public const int AdministerReleasePermissions = 512;
        public const int DeleteReleases = 1024;
    }
}
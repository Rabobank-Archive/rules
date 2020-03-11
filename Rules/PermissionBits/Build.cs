namespace SecurePipelineScan.Rules.PermissionBits
{
    public static class Build
    {
        public const int DeleteBuilds = 8;
        public const int DestroyBuilds = 32;
        public const int DeleteBuildDefinition = 4096;
        public const int AdministerBuildPermissions = 16384;
    }
}
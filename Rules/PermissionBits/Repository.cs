namespace SecurePipelineScan.Rules.PermissionBits
{
    public static class Repository
    {
        public const int BypassPoliciesPullRequest = 32768;
        public const int BypassPoliciesCodePush = 128;
        public const int ManagePermissions = 8192;
        public const int DeleteRepository = 512;
    }
}
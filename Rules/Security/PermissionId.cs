namespace SecurePipelineScan.Rules.Security
{
    static class PermissionId
    {
        public const int NotSet = 0;
        public const int Allow = 1;
        public const int AllowInherited = 3;
        public const int Deny = 4;
    }
}
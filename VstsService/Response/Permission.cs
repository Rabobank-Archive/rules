namespace SecurePipelineScan.VstsService.Response
{
    public class Permission
    {
        public int PermissionBit { get; set; }

        public string DisplayName { get; set; }

        public int PermissionId { get; set; }

        public string PermissionDisplayString { get; set; }
        public string NamespaceId { get; set; }
        public string PermissionToken { get; set; }
    }
}
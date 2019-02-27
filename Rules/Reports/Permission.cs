using Common;

namespace Rules.Reports
{
    public class Permission
    {
        public Permission(int permissionBit, PermissionId actualPermissionId)
        {
            PermissionBit = permissionBit;
            ActualPermissionId = actualPermissionId;
        }

        public Permission(int permissionBit)
        {
            PermissionBit = permissionBit;
        }

        public int PermissionBit { get; set; }
        public string Description { get; set; }

        public PermissionId? ActualPermissionId { get; set; }
        public PermissionId? ShouldBePermissionId { get; set; }

        public bool IsCompliant => (!ShouldBePermissionId.HasValue) ||
                                   (ActualPermissionId == ShouldBePermissionId) ||
                                   (ActualPermissionId == PermissionId.Deny &&
                                    ShouldBePermissionId == PermissionId.DenyInherited) ||
                                   (ActualPermissionId == PermissionId.DenyInherited &&
                                    ShouldBePermissionId == PermissionId.Deny) ||
                                   (ActualPermissionId == PermissionId.Allow &&
                                    ShouldBePermissionId == PermissionId.AllowInherited) ||
                                   (ActualPermissionId == PermissionId.AllowInherited &&
                                    ShouldBePermissionId == PermissionId.Allow);
    }
}
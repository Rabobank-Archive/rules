using SecurePipelineScan.VstsService.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Checks
{
    public static class Permission
    {
        public static bool HasNoPermissionToDeleteRepository (IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p =>    p.PermissionBit.ToString() == "512" && 
                                           p.PermissionId.ToString() == "2");
        }

        public static bool HasNoPermissionToManageRepositoryPermissions (IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p =>    p.PermissionBit.ToString() == "8192" &&
                                           p.PermissionId.ToString() == "2");
        }

    }
}

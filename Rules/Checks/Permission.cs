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
            return permissions.Any(p =>    p.PermissionBit == 512 && 
                                           (p.PermissionId == 2 || p.PermissionId == 4));
        }

        public static bool HasNoPermissionToManageRepositoryPermissions (IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p =>    p.PermissionBit == 8192 &&
                                           (p.PermissionId == 2 || p.PermissionId == 4));
        }

        public static bool HasNoPermissionToAdministerBuildPermissions (IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p =>    p.PermissionBit == 16384 &&
                                           (p.PermissionId == 2 || p.PermissionId == 4));
        }
        
        public static bool HasNoPermissionToDeleteBuildDefinition (IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p =>    p.PermissionBit == 4096 &&
                                           (p.PermissionId == 2 || p.PermissionId == 4));
        }

        public static bool HasNoPermissionToDeleteBuilds (IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p =>    p.PermissionBit == 8 &&
                                           (p.PermissionId == 2 || p.PermissionId == 4));
        }
 
        public static bool HasNoPermissionToDestroyBuilds (IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p =>    p.PermissionBit == 32 &&
                                           (p.PermissionId == 2 || p.PermissionId == 4));
        }

    }
}

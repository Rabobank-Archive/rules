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

        public static bool HasNoPermissionToAdministerReleasePermissions(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p =>    p.PermissionBit == 512 &&
                                           (p.PermissionId == 2 || p.PermissionId == 4));
        }
        
        public static bool HasNoPermissionToDeleteReleasePipeline(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p =>    p.PermissionBit == 4 &&
                                           (p.PermissionId == 2 || p.PermissionId == 4));
        }

        public static bool HasNoPermissionToDeleteReleases(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p =>    p.PermissionBit == 1024 &&
                                           (p.PermissionId == 2 || p.PermissionId == 4));
        }

        public static bool HasPermissionToManageReleaseApprovers(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p =>    p.PermissionBit == 8 &&
                                           (p.PermissionId == 1 || p.PermissionId == 3));
        }

        public static bool HasNoPermissionToManageReleaseApprovers(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p =>    p.PermissionBit == 8 &&
                                           (p.PermissionId == 2 || p.PermissionId == 4));
        }

        public static bool HasNoPermissionToCreateReleases(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p =>    p.PermissionBit == 64 &&
                                           (p.PermissionId == 2 || p.PermissionId == 4));
        }

        public static bool HasNoPermissionToDeleteTeamProject(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 4 && p.PermissionId == 2);
        }

        public static bool HasNoPermissionToPermanentlyDeleteWorkitems(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 32768 && p.PermissionId == 2);
        }

        public static bool HasNoPermissionToManageProjectProperties(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 131072 && p.PermissionId == 2);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Checks
{
    public static class Permission
    {
        private const int NotSet = 0;
        private const int Allow = 1;
        private const int Deny = 2;
        private const int AllowInherited = 3;
        private const int DenyInherited = 4;

        public static bool HasNoPermissionToDeleteRepository(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 512 &&
                                           (p.PermissionId == Deny || p.PermissionId == DenyInherited));
        }

        public static bool HasNoPermissionToManageRepositoryPermissions(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 8192 &&
                                           (p.PermissionId == Deny || p.PermissionId == DenyInherited));
        }

        public static bool HasNoPermissionToAdministerBuildPermissions(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 16384 &&
                                           (p.PermissionId == Deny || p.PermissionId == DenyInherited));
        }

        public static bool HasNoPermissionToDeleteBuildDefinition(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 4096 &&
                                           (p.PermissionId == Deny || p.PermissionId == DenyInherited));
        }

        public static bool HasNoPermissionToDeleteBuilds(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 8 &&
                                           (p.PermissionId == Deny || p.PermissionId == DenyInherited));
        }

        public static bool HasNoPermissionToDestroyBuilds(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 32 &&
                                           (p.PermissionId == Deny || p.PermissionId == DenyInherited));
        }

        public static bool HasNoPermissionToAdministerReleasePermissions(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 512 &&
                                           (p.PermissionId == Deny || p.PermissionId == DenyInherited));
        }

        public static bool HasNoPermissionToDeleteReleasePipeline(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 4 &&
                                           (p.PermissionId == Deny || p.PermissionId == DenyInherited));
        }

        public static bool HasNoPermissionToDeleteReleases(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 1024 &&
                                           (p.PermissionId == Deny || p.PermissionId == DenyInherited));
        }

        public static bool HasPermissionToManageReleaseApprovers(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 8 &&
                                           (p.PermissionId == Allow || p.PermissionId == 3));
        }

        public static bool HasNotSetToManageReleaseApprovers(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 8 &&
                                           (p.PermissionId == NotSet || p.PermissionId == AllowInherited || p.PermissionId == DenyInherited));
        }

        public static bool HasNoPermissionToManageReleaseApprovers(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 8 &&
                                           (p.PermissionId == Deny || p.PermissionId == DenyInherited));
        }

        public static bool HasNoPermissionToCreateReleases(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 64 &&
                                           (p.PermissionId == Deny || p.PermissionId == DenyInherited));
        }

        public static bool HasPermissionToCreateReleases(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 64 &&
                                           (p.PermissionId == Allow || p.PermissionId == AllowInherited));
        }

        public static bool HasNoPermissionToDeleteTeamProject(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 4 && p.PermissionId == Deny);
        }

        public static bool HasNoPermissionToPermanentlyDeleteWorkitems(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 32768 && p.PermissionId == Deny);
        }

        public static bool HasNoPermissionToManageProjectProperties(IEnumerable<Response.Permission> permissions)
        {
            return permissions.Any(p => p.PermissionBit == 131072 && p.PermissionId == Deny);
        }
    }
}
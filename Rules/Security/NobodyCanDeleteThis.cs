using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using SecurePipelineScan.VstsService.Requests;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;
using System.Collections.Generic;
using System.Linq;

namespace SecurePipelineScan.Rules.Security
{
    public abstract class NobodyCanDeleteThis
    {
        protected abstract string PermissionsDisplayName { get; }
        protected abstract int[] AllowedPermissions { get; }

        public bool Evaluate(string projectId, string id)
        {
            var groups = WhichGroups(projectId, id);
            var permissions = WhichPermissions(projectId, id, groups);
            return permissions.All(p => p.DisplayName != PermissionsDisplayName || AllowedPermissions.Contains(p.PermissionId));
        }

        protected abstract IEnumerable<Permission> WhichPermissions(string projectId, string id, IEnumerable<ApplicationGroup> groups);
        protected abstract PermissionsSetId WhichPermissions(string projectId, string id, ApplicationGroup group);
        protected abstract IEnumerable<ApplicationGroup> WhichGroups(string projectId, string id);


        public void Reconcile(string projectId, string id)
        {
            var groups = WhichGroups(projectId, id);
            foreach (var group in groups)
            {
                var permissionSetId = WhichPermissions(projectId, id, group);
                var permission = permissionSetId.Permissions.Single(p => p.DisplayName == PermissionsDisplayName);

                if (!AllowedPermissions.Contains(permission.PermissionId))
                {
                    UpdatePermissionToDeny(projectId, group, permissionSetId, permission);
                }
            }
        }

        protected abstract void UpdatePermissionToDeny(string projectId, ApplicationGroup group, PermissionsSetId permissionSetId, Permission permission);
    }
}
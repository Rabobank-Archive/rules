using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using SecurePipelineScan.VstsService.Requests;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;
using System.Collections.Generic;
using System.Linq;

namespace SecurePipelineScan.Rules.Security
{
    public abstract class NobodyCanDeleteThisBase
    {
        protected abstract string PermissionsDisplayName { get; }
        protected abstract IEnumerable<string> IgnoredIdentitiesDisplayNames { get; }
        protected abstract IEnumerable<int> AllowedPermissions { get; }

        protected abstract PermissionsSetId LoadPermissionsSetForGroup(string projectId, string id, ApplicationGroup group);
        protected abstract IEnumerable<ApplicationGroup> LoadGroups(string projectId, string id);
        protected abstract void UpdatePermissionToDeny(string projectId, ApplicationGroup group, PermissionsSetId permissionSetId, Permission permission);

        public bool Evaluate(string projectId, string id)
        {
            var groups = LoadGroups(projectId, id)
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            var permissions = groups.SelectMany(g => LoadPermissionsSetForGroup(projectId, id, g).Permissions);
            return permissions.All(p => p.DisplayName != PermissionsDisplayName || AllowedPermissions.Contains(p.PermissionId));
        }

        public void Reconcile(string projectId, string id)
        {
            var groups = LoadGroups(projectId, id)
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            foreach (var group in groups)
            {
                var permissionSetId = LoadPermissionsSetForGroup(projectId, id, group);
                var permission = permissionSetId.Permissions.Single(p => p.DisplayName.Trim() == PermissionsDisplayName);

                if (!AllowedPermissions.Contains(permission.PermissionId))
                {
                    permission.PermissionId = PermissionId.Deny;
                    UpdatePermissionToDeny(projectId, group, permissionSetId, permission);
                }
            }
        }
    }
}
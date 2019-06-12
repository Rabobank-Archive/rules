using SecurePipelineScan.VstsService.Response;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public abstract class RuleBase
    {
        protected abstract int PermissionBit { get; }
        protected abstract IEnumerable<string> IgnoredIdentitiesDisplayNames { get; }
        protected abstract IEnumerable<int> AllowedPermissions { get; }

        protected abstract Task<PermissionsSetId> LoadPermissionsSetForGroup(string projectId, string id,
            ApplicationGroup @group);
        protected abstract Task<IEnumerable<ApplicationGroup>> LoadGroups(string projectId, string id);
        protected abstract void UpdatePermissionToDeny(string projectId, ApplicationGroup group, PermissionsSetId permissionSetId, Permission permission);

        public bool Evaluate(string projectId, string id)
        {
            var groups = LoadGroups(projectId, id)
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            var permissions = groups.SelectMany(g => LoadPermissionsSetForGroup(projectId, id, g).Permissions);
            return permissions.All(p => p.PermissionBit != PermissionBit || AllowedPermissions.Contains(p.PermissionId));
        }

        public void Reconcile(string projectId, string id)
        {
            var groups = LoadGroups(projectId, id)
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            foreach (var group in groups)
            {
                var permissionSetId = LoadPermissionsSetForGroup(projectId, id, @group);
                var permission = permissionSetId.Permissions.Single(p => p.PermissionBit == PermissionBit);

                if (!AllowedPermissions.Contains(permission.PermissionId))
                {
                    permission.PermissionId = PermissionId.Deny;
                    UpdatePermissionToDeny(projectId, group, permissionSetId, permission);
                }
            }
        }
    }
}
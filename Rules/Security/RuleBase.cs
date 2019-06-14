using SecurePipelineScan.VstsService.Response;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Security
{
    public abstract class RuleBase
    {
        protected abstract string NamespaceId { get; }
        protected abstract IEnumerable<int> PermissionBits { get; }
        protected abstract IEnumerable<string> IgnoredIdentitiesDisplayNames { get; }
        protected abstract IEnumerable<int> AllowedPermissions { get; }

        protected abstract Task<PermissionsSetId> LoadPermissionsSetForGroup(string projectId, string id,
            ApplicationGroup @group);
        protected abstract Task<IEnumerable<ApplicationGroup>> LoadGroups(string projectId, string id);
        protected abstract Task UpdatePermissionToDeny(string projectId, ApplicationGroup group, PermissionsSetId permissionSetId, Permission permission);

        public async Task<bool> Evaluate(string projectId, string id)
        {
            var groups = (await LoadGroups(projectId, id))
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            var permissions =
                (await Task.WhenAll(groups.Select(g => LoadPermissionsSetForGroup(projectId, id, g)))).SelectMany(p =>
                    p.Permissions);
            return permissions.All(p => !PermissionBits.Contains(p.PermissionBit) || AllowedPermissions.Contains(p.PermissionId));
        }

        public async Task Reconcile(string projectId, string id)
        {
            var groups = (await LoadGroups(projectId, id))
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            foreach (var group in groups)
            {
                var permissionSetId = await LoadPermissionsSetForGroup(projectId, id, group);
                var permissions = permissionSetId.Permissions
                    .Where(p => PermissionBits.Contains(p.PermissionBit) && !AllowedPermissions.Contains(p.PermissionId));

                foreach (var permission in permissions)
                {
                    permission.PermissionId = PermissionId.Deny;
                    await UpdatePermissionToDeny(projectId, group, permissionSetId, permission);
                }
            }
        }
    }
}
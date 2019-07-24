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

        protected abstract Task<PermissionsSetId> LoadPermissionsSetForGroupAsync(string projectId, string id,
            ApplicationGroup group);
        protected abstract Task<IEnumerable<ApplicationGroup>> LoadGroupsAsync(string projectId, string id);
        protected abstract Task UpdatePermissionAsync(string projectId, ApplicationGroup group, PermissionsSetId permissionSetId, Permission permission);

        public virtual async Task<bool> EvaluateAsync(string projectId, string id)
        {
            var groups = (await LoadGroupsAsync(projectId, id).ConfigureAwait(false))
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            var permissions =
                (await Task.WhenAll(groups.Select(g => LoadPermissionsSetForGroupAsync(projectId, id, g))).ConfigureAwait(false))
                    .SelectMany(p => p.Permissions);
            return permissions.All(p => !PermissionBits.Contains(p.PermissionBit) || AllowedPermissions.Contains(p.PermissionId));
        }

        public virtual async Task ReconcileAsync(string projectId, string id)
        {
            var groups = (await LoadGroupsAsync(projectId, id).ConfigureAwait(false))
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            foreach (var group in groups)
            {
                var permissionSetId = await LoadPermissionsSetForGroupAsync(projectId, id, group)
                    .ConfigureAwait(false);
                var permissions = permissionSetId.Permissions
                    .Where(p => PermissionBits.Contains(p.PermissionBit) && !AllowedPermissions.Contains(p.PermissionId));

                foreach (var permission in permissions)
                {
                    permission.PermissionId = PermissionId.Deny;
                    await UpdatePermissionAsync(projectId, group, permissionSetId, permission)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}
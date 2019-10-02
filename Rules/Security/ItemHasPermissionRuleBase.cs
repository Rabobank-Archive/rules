using Response = SecurePipelineScan.VstsService.Response;
using SecurePipelineScan.VstsService.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using System;

namespace SecurePipelineScan.Rules.Security
{
    public abstract class ItemHasPermissionRuleBase
    {
        readonly IVstsRestClient _client;

        protected ItemHasPermissionRuleBase(IVstsRestClient client)
        {
            _client = client;
        }

        protected abstract string NamespaceId { get; }
        protected abstract IEnumerable<int> PermissionBits { get; }
        protected abstract IEnumerable<string> IgnoredIdentitiesDisplayNames { get; }
        protected abstract IEnumerable<int> AllowedPermissions { get; }

        public virtual async Task<bool> EvaluateAsync(string projectId, string itemId, string scope)
        {
            var groups = (await LoadGroupsAsync(projectId, itemId, scope)
                .ConfigureAwait(false))
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            return (await Task.WhenAll(groups.Select(g => LoadPermissionsSetForGroupAsync(
                    projectId, itemId, g, scope)))
                .ConfigureAwait(false))
                .SelectMany(p => p.Permissions)
                .All(p => !PermissionBits.Contains(p.PermissionBit) 
                    || AllowedPermissions.Contains(p.PermissionId));
        }

        public virtual async Task ReconcileAsync(string projectId, string itemId, string scope)
        {
            var groups = (await LoadGroupsAsync(projectId, itemId, scope)
                .ConfigureAwait(false))
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            foreach (var group in groups)
            {
                var permissionSetId = await LoadPermissionsSetForGroupAsync(projectId, 
                        itemId, group, scope)
                    .ConfigureAwait(false);
                var permissions = permissionSetId.Permissions
                    .Where(p => PermissionBits.Contains(p.PermissionBit) 
                        && !AllowedPermissions.Contains(p.PermissionId));

                foreach (var permission in permissions)
                {
                    permission.PermissionId = PermissionId.Deny;
                    await UpdatePermissionAsync(projectId, group, permissionSetId, permission)
                        .ConfigureAwait(false);
                }
            }
        }

        protected async Task<IEnumerable<Response.ApplicationGroup>> LoadGroupsAsync(string projectId)
        {
            return (await _client.GetAsync(ApplicationGroup.ApplicationGroups(projectId))
                .ConfigureAwait(false))
                .Identities;
        }

        protected async Task<IEnumerable<Response.ApplicationGroup>> LoadGroupsAsync(string projectId,
            string itemId, string scope)
        {
            if (scope == RuleScopes.Repositories)
                return (await _client.GetAsync(ApplicationGroup.ExplicitIdentitiesRepos(projectId,
                        NamespaceId, itemId))
                    .ConfigureAwait(false))
                    .Identities;

            return (await _client.GetAsync(ApplicationGroup.ExplicitIdentitiesPipelines(projectId,
                        NamespaceId, itemId))
                    .ConfigureAwait(false))
                    .Identities;
        }

        protected async Task<Response.PermissionsSetId> LoadPermissionsSetForGroupAsync(string projectId,
            Response.ApplicationGroup group, string scope)
        {
            if (scope == RuleScopes.GlobalPermissions)
                return (await _client.GetAsync(Permissions.PermissionsGroupProjectId(projectId,
                    group.TeamFoundationId))
                .ConfigureAwait(false))
                .Security;

            return await _client.GetAsync(Permissions.PermissionsGroupSetId(projectId, NamespaceId,
                    group.TeamFoundationId))
                .ConfigureAwait(false);
        }

        protected async Task<Response.PermissionsSetId> LoadPermissionsSetForGroupAsync(string projectId,
            string itemId, Response.ApplicationGroup group, string scope)
        {
            if (scope == RuleScopes.Repositories)
                return await _client.GetAsync(Permissions.PermissionsGroupRepository(projectId, NamespaceId,
                    group.TeamFoundationId, itemId))
                .ConfigureAwait(false);

            return await _client.GetAsync(Permissions.PermissionsGroupSetIdDefinition(projectId, NamespaceId,
                   group.TeamFoundationId, itemId))
               .ConfigureAwait(false);
        }

        protected async Task UpdatePermissionAsync(string projectId, Response.ApplicationGroup group,
            Response.PermissionsSetId permissionSetId, Response.Permission permission)
        {
            await _client.PostAsync(Permissions.ManagePermissions(projectId), 
                    new Permissions.ManagePermissionsData(group.TeamFoundationId, 
                    permissionSetId.DescriptorIdentifier, permissionSetId.DescriptorIdentityType, permission)
                .Wrap())
                .ConfigureAwait(false);
        }
    }
}
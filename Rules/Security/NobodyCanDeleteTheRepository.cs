using System.Collections.Generic;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;
using PermissionsSetId = SecurePipelineScan.VstsService.Response.PermissionsSetId;
using System;
using System.Linq;

namespace SecurePipelineScan.Rules.Security
{
    public class NobodyCanDeleteTheRepository : IRepositoryRule, IReconcile
    {
        private readonly IVstsRestClient _client;

        const int PermissionBitDeletRepository = 512;
        const int PermissionBitManagePermissions = 8192;

        private static string NamespaceId => "2e9eb7ed-3c0a-47d4-87c1-0ffdd275fd87"; //Git Repositories
        private static IEnumerable<int> PermissionBits => new[]
        {
            PermissionBitDeletRepository,
            PermissionBitManagePermissions
        };
        private static IEnumerable<int> AllowedPermissions => new[]
        {
            PermissionId.NotSet,
            PermissionId.Deny,
            PermissionId.DenyInherited
        };
        private static IEnumerable<string> IgnoredIdentitiesDisplayNames => new[]
        {
            "Project Collection Administrators",
            "Project Collection Service Accounts"
        };

        string IRepositoryRule.Description => "Nobody can delete the repository";
        string IRepositoryRule.Why => "To enforce auditability, no data should be deleted. " +
            "Therefore, nobody should be able to delete the repository.";
        bool IRepositoryRule.IsSox => true;
        string[] IReconcile.Impact => new[]
        {
            "For all security groups the 'Delete Repository' permission is set to Deny",
            "For all security groups the 'Manage Permissions' permission is set to Deny"
        };

        public NobodyCanDeleteTheRepository(IVstsRestClient client)
        {
            _client = client;
        }

        public async Task<bool> EvaluateAsync(string projectId, string repositoryId,
            IEnumerable<Response.MinimumNumberOfReviewersPolicy> policies)
        {
            var groups = (await LoadGroupsAsync(projectId, repositoryId).ConfigureAwait(false))
                .Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            return (await Task.WhenAll(groups
                .Select(g => LoadPermissionsSetForGroupAsync(projectId, repositoryId, g))).ConfigureAwait(false))
                .SelectMany(p => p.Permissions)
                .All(p => !PermissionBits.Contains(p.PermissionBit) || AllowedPermissions.Contains(p.PermissionId));
        }

        public async Task ReconcileAsync(string projectId, string id)
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

        private async Task<IEnumerable<ApplicationGroup>> LoadGroupsAsync(string projectId, string repositoryId) =>
            (await _client.GetAsync(VstsService.Requests.ApplicationGroup.ExplicitIdentitiesRepos(projectId, NamespaceId, repositoryId))
                .ConfigureAwait(false))
                .Identities;

        private Task<PermissionsSetId> LoadPermissionsSetForGroupAsync(string projectId, string repositoryId,
            ApplicationGroup group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            return LoadPermissionsSetForGroupInternalAsync(projectId, repositoryId, group);
        }

        private async Task<PermissionsSetId> LoadPermissionsSetForGroupInternalAsync(string projectId, string repositoryId, ApplicationGroup group)
        {
            return 
                await _client.GetAsync(Permissions.PermissionsGroupRepository(projectId, NamespaceId, group.TeamFoundationId, repositoryId))
                    .ConfigureAwait(false);
        }

        private Task UpdatePermissionAsync(string projectId, ApplicationGroup group,
            PermissionsSetId permissionSetId, Response.Permission permission)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));
            if (permissionSetId == null)
                throw new ArgumentNullException(nameof(permissionSetId));

            return UpdatePermissionInternalAsync(projectId, group, permissionSetId, permission);
        }

        private async Task UpdatePermissionInternalAsync(string projectId, ApplicationGroup group, PermissionsSetId permissionSetId, Response.Permission permission)
        {
            await _client.PostAsync(Permissions.ManagePermissions(projectId),
                new Permissions.ManagePermissionsData(group.TeamFoundationId, permissionSetId.DescriptorIdentifier,
                    permissionSetId.DescriptorIdentityType, permission).Wrap())
                .ConfigureAwait(false);
        }
    }
}
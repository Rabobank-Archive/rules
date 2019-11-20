using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public sealed class NobodyCanDeleteTheRepository : IRepositoryRule, IReconcile
    {
        private readonly IVstsRestClient _client;

        public NobodyCanDeleteTheRepository(IVstsRestClient client)
        {
            _client = client;
        }

        private const int PermissionBitDeleteRepository = 512;
        private const int PermissionBitManagePermissions = 8192;

        private static string NamespaceId => "2e9eb7ed-3c0a-47d4-87c1-0ffdd275fd87"; //Git Repositories

        private static IEnumerable<int> PermissionBits => new[]
        {
            PermissionBitDeleteRepository,
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

        string IRule.Description => "Nobody can delete the repository (SOx)";
        string IRule.Link => "https://confluence.dev.somecompany.nl/x/RI8AD";
        bool IRule.IsSox => true;
        bool IReconcile.RequiresStageId => false;

        string[] IReconcile.Impact => new[]
        {
            "For all security groups the 'Delete Repository' permission is set to Deny",
            "For all security groups the 'Manage Permissions' permission is set to Deny"
        };

        public async Task<bool> EvaluateAsync(string projectId, string repositoryId, IEnumerable<Response.MinimumNumberOfReviewersPolicy> policies)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (repositoryId == null)
                throw new ArgumentNullException(nameof(repositoryId));

            var groups = (await _client.GetAsync(ApplicationGroup.ExplicitIdentitiesRepos(projectId, NamespaceId, repositoryId)).ConfigureAwait(false))
                .Identities.Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            var permissionsSetIds = await Task.WhenAll(
                groups.Select(g => _client.GetAsync(Permissions.PermissionsGroupRepository(projectId, NamespaceId, g.TeamFoundationId, repositoryId)))).ConfigureAwait(false);
            
            return permissionsSetIds
                .SelectMany(p => p.Permissions)
                .All(p => !PermissionBits.Contains(p.PermissionBit) || AllowedPermissions.Contains(p.PermissionId));
        }

        public async Task ReconcileAsync(string projectId, string itemId, string stageId)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (itemId == null)
                throw new ArgumentNullException(nameof(itemId));

            var groups = (await _client.GetAsync(ApplicationGroup.ExplicitIdentitiesRepos(projectId, NamespaceId, itemId)).ConfigureAwait(false))
                .Identities.Where(g => !IgnoredIdentitiesDisplayNames.Contains(g.FriendlyDisplayName));

            foreach (var group in groups)
            {
                var permissionSetId = await _client.GetAsync(Permissions.PermissionsGroupRepository(projectId, NamespaceId, group.TeamFoundationId, itemId)).ConfigureAwait(false);
                var permissions = permissionSetId.Permissions.Where(p => PermissionBits.Contains(p.PermissionBit)
                                                                         && !AllowedPermissions.Contains(p.PermissionId));

                foreach (var permission in permissions)
                {
                    permission.PermissionId = PermissionId.Deny;
                    await _client.PostAsync(
                            Permissions.ManagePermissions(projectId),
                            new Permissions.ManagePermissionsData(group.TeamFoundationId, permissionSetId.DescriptorIdentifier, permissionSetId.DescriptorIdentityType, permission).Wrap())
                        .ConfigureAwait(false);
                }
            }
        }
    }
}